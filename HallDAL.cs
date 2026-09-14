using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CinemaHallSystem.Models;

namespace CinemaHallSystem.DAL
{
    public class HallDAL : IRepository<Hall>
    {
        public int Add(Hall entity)
        {
            using var conn = DBConnection.GetConnection();
            string query = "INSERT INTO Halls (HallName, TotalSeats) VALUES (@HallName, @TotalSeats); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@HallName", entity.HallName);
            cmd.Parameters.AddWithValue("@TotalSeats", entity.TotalSeats);
            conn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Update(Hall entity)
        {
            using var conn = DBConnection.GetConnection();
            string query = "UPDATE Halls SET HallName=@HallName, TotalSeats=@TotalSeats WHERE HallID=@HallID";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@HallID", entity.HallID);
            cmd.Parameters.AddWithValue("@HallName", entity.HallName);
            cmd.Parameters.AddWithValue("@TotalSeats", entity.TotalSeats);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = DBConnection.GetConnection();
            string query = "DELETE FROM Halls WHERE HallID=@id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public Hall GetById(int id)
        {
            using var conn = DBConnection.GetConnection();
            string query = "SELECT * FROM Halls WHERE HallID=@id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read()) return MapFromReader(reader);
            return null;
        }

        public List<Hall> GetAll()
        {
            var list = new List<Hall>();
            using var conn = DBConnection.GetConnection();
            string query = "SELECT * FROM Halls";
            using var cmd = new SqlCommand(query, conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(MapFromReader(reader));
            return list;
        }

        private Hall MapFromReader(SqlDataReader reader)
        {
            return new Hall
            {
                HallID = Convert.ToInt32(reader["HallID"]),
                HallName = reader["HallName"].ToString(),
                TotalSeats = Convert.ToInt32(reader["TotalSeats"])
            };
        }
    }
}
