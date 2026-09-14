using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CinemaHallSystem.Models;

namespace CinemaHallSystem.DAL
{
    public class ShowSeatDAL : IRepository<ShowSeat>
    {
        public int Add(ShowSeat entity)
        {
            using var conn = DBConnection.GetConnection();
            string query = "INSERT INTO ShowSeats (ShowID, SeatID, Status) VALUES (@ShowID, @SeatID, @Status); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ShowID", entity.ShowID);
            cmd.Parameters.AddWithValue("@SeatID", entity.SeatID);
            cmd.Parameters.AddWithValue("@Status", entity.Status);
            conn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Update(ShowSeat entity)
        {
            using var conn = DBConnection.GetConnection();
            string query = "UPDATE ShowSeats SET ShowID=@ShowID, SeatID=@SeatID, Status=@Status WHERE ShowSeatID=@ShowSeatID";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ShowSeatID", entity.ShowSeatID);
            cmd.Parameters.AddWithValue("@ShowID", entity.ShowID);
            cmd.Parameters.AddWithValue("@SeatID", entity.SeatID);
            cmd.Parameters.AddWithValue("@Status", entity.Status);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = DBConnection.GetConnection();
            string query = "DELETE FROM ShowSeats WHERE ShowSeatID=@id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public ShowSeat GetById(int id)
        {
            using var conn = DBConnection.GetConnection();
            string query = "SELECT ss.*, s.SeatRow, s.SeatNo, s.SeatType FROM ShowSeats ss JOIN Seats s ON ss.SeatID = s.SeatID WHERE ss.ShowSeatID=@id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read()) return MapFromReader(reader);
            return null;
        }

        public List<ShowSeat> GetAll()
        {
            throw new NotImplementedException();
        }

        public List<ShowSeat> GetByShowId(int showId)
        {
            var list = new List<ShowSeat>();
            using var conn = DBConnection.GetConnection();
            string query = "SELECT ss.*, s.SeatRow, s.SeatNo, s.SeatType FROM ShowSeats ss JOIN Seats s ON ss.SeatID = s.SeatID WHERE ss.ShowID=@ShowID ORDER BY s.SeatRow, s.SeatNo";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ShowID", showId);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(MapFromReader(reader));
            return list;
        }

        public void GenerateShowSeats(int showId, int hallId)
        {
            using var conn = DBConnection.GetConnection();
            string query = "INSERT INTO ShowSeats (ShowID, SeatID, Status) SELECT @ShowID, SeatID, 'Available' FROM Seats WHERE HallID = @HallID";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ShowID", showId);
            cmd.Parameters.AddWithValue("@HallID", hallId);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public int LockSeat(int showSeatId)
        {
            using var conn = DBConnection.GetConnection();
            string query = "UPDATE ShowSeats SET Status='Locked' WHERE ShowSeatID=@id AND Status='Available'";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", showSeatId);
            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        public void UpdateStatus(int showSeatId, string status)
        {
            using var conn = DBConnection.GetConnection();
            string query = "UPDATE ShowSeats SET Status=@Status WHERE ShowSeatID=@id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", showSeatId);
            cmd.Parameters.AddWithValue("@Status", status);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void UpdateStatusBatch(List<int> showSeatIds, string status, SqlConnection conn, SqlTransaction tran)
        {
            foreach (int id in showSeatIds)
            {
                string query = "UPDATE ShowSeats SET Status=@Status WHERE ShowSeatID=@id";
                using var cmd = new SqlCommand(query, conn, tran);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.ExecuteNonQuery();
            }
        }

        private ShowSeat MapFromReader(SqlDataReader reader)
        {
            var ss = new ShowSeat
            {
                ShowSeatID = Convert.ToInt32(reader["ShowSeatID"]),
                ShowID = Convert.ToInt32(reader["ShowID"]),
                SeatID = Convert.ToInt32(reader["SeatID"]),
                Status = reader["Status"].ToString()
            };

            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals("SeatRow", StringComparison.OrdinalIgnoreCase))
                    ss.SeatRow = reader["SeatRow"].ToString();
                if (reader.GetName(i).Equals("SeatNo", StringComparison.OrdinalIgnoreCase))
                    ss.SeatNo = Convert.ToInt32(reader["SeatNo"]);
                if (reader.GetName(i).Equals("SeatType", StringComparison.OrdinalIgnoreCase))
                    ss.SeatType = reader["SeatType"].ToString();
            }
            return ss;
        }
    }
}
