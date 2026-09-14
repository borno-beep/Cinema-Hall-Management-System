using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CinemaHallSystem.Models;

namespace CinemaHallSystem.DAL
{
    public class MovieDAL : IRepository<Movie>
    {
        public int Add(Movie entity)
        {
            using var conn = DBConnection.GetConnection();
            string query = "INSERT INTO Movies (Title, Genre, DurationMinutes, Language, AgeRating, ReleaseDate, Description) VALUES (@Title, @Genre, @DurationMinutes, @Language, @AgeRating, @ReleaseDate, @Description); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Title", entity.Title);
            cmd.Parameters.AddWithValue("@Genre", entity.Genre);
            cmd.Parameters.AddWithValue("@DurationMinutes", entity.DurationMinutes);
            cmd.Parameters.AddWithValue("@Language", entity.Language);
            cmd.Parameters.AddWithValue("@AgeRating", entity.AgeRating);
            cmd.Parameters.AddWithValue("@ReleaseDate", entity.ReleaseDate);
            cmd.Parameters.AddWithValue("@Description", string.IsNullOrEmpty(entity.Description) ? (object)DBNull.Value : entity.Description);
            conn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Update(Movie entity)
        {
            using var conn = DBConnection.GetConnection();
            string query = "UPDATE Movies SET Title=@Title, Genre=@Genre, DurationMinutes=@DurationMinutes, Language=@Language, AgeRating=@AgeRating, ReleaseDate=@ReleaseDate, Description=@Description WHERE MovieID=@MovieID";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@MovieID", entity.MovieID);
            cmd.Parameters.AddWithValue("@Title", entity.Title);
            cmd.Parameters.AddWithValue("@Genre", entity.Genre);
            cmd.Parameters.AddWithValue("@DurationMinutes", entity.DurationMinutes);
            cmd.Parameters.AddWithValue("@Language", entity.Language);
            cmd.Parameters.AddWithValue("@AgeRating", entity.AgeRating);
            cmd.Parameters.AddWithValue("@ReleaseDate", entity.ReleaseDate);
            cmd.Parameters.AddWithValue("@Description", string.IsNullOrEmpty(entity.Description) ? (object)DBNull.Value : entity.Description);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = DBConnection.GetConnection();
            string query = "DELETE FROM Movies WHERE MovieID=@id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public Movie GetById(int id)
        {
            using var conn = DBConnection.GetConnection();
            string query = "SELECT * FROM Movies WHERE MovieID=@id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read()) return MapFromReader(reader);
            return null;
        }

        public List<Movie> GetAll()
        {
            var list = new List<Movie>();
            using var conn = DBConnection.GetConnection();
            string query = "SELECT * FROM Movies";
            using var cmd = new SqlCommand(query, conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(MapFromReader(reader));
            return list;
        }

        public List<Movie> Search(string keyword)
        {
            var list = new List<Movie>();
            using var conn = DBConnection.GetConnection();
            string query = "SELECT * FROM Movies WHERE Title LIKE @kw OR Genre LIKE @kw";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(MapFromReader(reader));
            return list;
        }

        private Movie MapFromReader(SqlDataReader reader)
        {
            return new Movie
            {
                MovieID = reader["MovieID"] != DBNull.Value ? Convert.ToInt32(reader["MovieID"]) : 0,
                Title = reader["Title"] != DBNull.Value ? reader["Title"].ToString() : "",
                Genre = reader["Genre"] != DBNull.Value ? reader["Genre"].ToString() : "",
                DurationMinutes = reader["DurationMinutes"] != DBNull.Value ? Convert.ToInt32(reader["DurationMinutes"]) : 0,
                Language = reader["Language"] != DBNull.Value ? reader["Language"].ToString() : "",
                AgeRating = reader["AgeRating"] != DBNull.Value ? reader["AgeRating"].ToString() : "",
                ReleaseDate = reader["ReleaseDate"] != DBNull.Value ? Convert.ToDateTime(reader["ReleaseDate"]) : DateTime.Today,
                Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : ""
            };
        }
    }
}
