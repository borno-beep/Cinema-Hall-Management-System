using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CinemaHallSystem.Models;

namespace CinemaHallSystem.DAL
{
    public class ShowDAL : IRepository<Show>
    {
        public int Add(Show entity)
        {
            using var conn = DBConnection.GetConnection();
            string query = "INSERT INTO Shows (MovieID, HallID, ShowDate, ShowTime, TicketPrice) VALUES (@MovieID, @HallID, @ShowDate, @ShowTime, @TicketPrice); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@MovieID", entity.MovieID);
            cmd.Parameters.AddWithValue("@HallID", entity.HallID);
            cmd.Parameters.AddWithValue("@ShowDate", entity.ShowDate);
            cmd.Parameters.AddWithValue("@ShowTime", entity.ShowTime);
            cmd.Parameters.AddWithValue("@TicketPrice", entity.TicketPrice);
            conn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Update(Show entity)
        {
            using var conn = DBConnection.GetConnection();
            string query = "UPDATE Shows SET MovieID=@MovieID, HallID=@HallID, ShowDate=@ShowDate, ShowTime=@ShowTime, TicketPrice=@TicketPrice WHERE ShowID=@ShowID";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ShowID", entity.ShowID);
            cmd.Parameters.AddWithValue("@MovieID", entity.MovieID);
            cmd.Parameters.AddWithValue("@HallID", entity.HallID);
            cmd.Parameters.AddWithValue("@ShowDate", entity.ShowDate);
            cmd.Parameters.AddWithValue("@ShowTime", entity.ShowTime);
            cmd.Parameters.AddWithValue("@TicketPrice", entity.TicketPrice);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = DBConnection.GetConnection();
            string query = "DELETE FROM Shows WHERE ShowID=@id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public Show GetById(int id)
        {
            using var conn = DBConnection.GetConnection();
            string query = "SELECT s.*, m.Title AS MovieTitle, h.HallName FROM Shows s JOIN Movies m ON s.MovieID = m.MovieID JOIN Halls h ON s.HallID = h.HallID WHERE s.ShowID=@id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read()) return MapFromReader(reader);
            return null;
        }

        public List<Show> GetAll()
        {
            var list = new List<Show>();
            using var conn = DBConnection.GetConnection();
            string query = "SELECT s.*, m.Title AS MovieTitle, h.HallName FROM Shows s JOIN Movies m ON s.MovieID = m.MovieID JOIN Halls h ON s.HallID = h.HallID";
            using var cmd = new SqlCommand(query, conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(MapFromReader(reader));
            return list;
        }

        public List<Show> GetUpcoming()
        {
            var list = new List<Show>();
            using var conn = DBConnection.GetConnection();
            string query = "SELECT s.*, m.Title AS MovieTitle, h.HallName FROM Shows s JOIN Movies m ON s.MovieID = m.MovieID JOIN Halls h ON s.HallID = h.HallID WHERE s.ShowDate >= CAST(GETDATE() AS DATE) ORDER BY s.ShowDate, s.ShowTime";
            using var cmd = new SqlCommand(query, conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(MapFromReader(reader));
            return list;
        }

        public List<Show> GetByMovieId(int movieId)
        {
            var list = new List<Show>();
            using var conn = DBConnection.GetConnection();
            string query = "SELECT s.*, m.Title AS MovieTitle, h.HallName FROM Shows s JOIN Movies m ON s.MovieID = m.MovieID JOIN Halls h ON s.HallID = h.HallID WHERE s.MovieID=@MovieID";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@MovieID", movieId);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(MapFromReader(reader));
            return list;
        }

        public List<Show> GetByDate(DateTime date)
        {
            var list = new List<Show>();
            using var conn = DBConnection.GetConnection();
            string query = "SELECT s.*, m.Title AS MovieTitle, h.HallName FROM Shows s JOIN Movies m ON s.MovieID = m.MovieID JOIN Halls h ON s.HallID = h.HallID WHERE s.ShowDate=@ShowDate";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ShowDate", date.Date);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(MapFromReader(reader));
            return list;
        }

        public bool HasConflict(int hallId, DateTime showDate, TimeSpan showTime, int durationMinutes, int excludeShowId = 0)
        {
            using var conn = DBConnection.GetConnection();
            string query = @"SELECT s.ShowTime, m.DurationMinutes 
                             FROM Shows s 
                             JOIN Movies m ON s.MovieID = m.MovieID 
                             WHERE s.HallID = @HallID AND s.ShowDate = @ShowDate AND s.ShowID != @ExcludeID";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@HallID", hallId);
            cmd.Parameters.AddWithValue("@ShowDate", showDate.Date);
            cmd.Parameters.AddWithValue("@ExcludeID", excludeShowId);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            
            TimeSpan newStart = showTime;
            TimeSpan newEnd = showTime.Add(TimeSpan.FromMinutes(durationMinutes));

            while (reader.Read())
            {
                TimeSpan existingStart = (TimeSpan)reader["ShowTime"];
                int existingDur = Convert.ToInt32(reader["DurationMinutes"]);
                TimeSpan existingEnd = existingStart.Add(TimeSpan.FromMinutes(existingDur));

                if (newStart < existingEnd && newEnd > existingStart)
                {
                    return true;
                }
            }
            return false;
        }

        private Show MapFromReader(SqlDataReader reader)
        {
            var show = new Show
            {
                ShowID = Convert.ToInt32(reader["ShowID"]),
                MovieID = Convert.ToInt32(reader["MovieID"]),
                HallID = Convert.ToInt32(reader["HallID"]),
                ShowDate = Convert.ToDateTime(reader["ShowDate"]),
                ShowTime = (TimeSpan)reader["ShowTime"],
                TicketPrice = Convert.ToDecimal(reader["TicketPrice"])
            };
            
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals("MovieTitle", StringComparison.OrdinalIgnoreCase) && !reader.IsDBNull(i))
                    show.MovieTitle = reader["MovieTitle"].ToString();
                if (reader.GetName(i).Equals("HallName", StringComparison.OrdinalIgnoreCase) && !reader.IsDBNull(i))
                    show.HallName = reader["HallName"].ToString();
            }

            return show;
        }
    }
}
