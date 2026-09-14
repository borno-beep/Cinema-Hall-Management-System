using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CinemaHallSystem.Models;

namespace CinemaHallSystem.DAL
{
    public class SeatDAL : IRepository<Seat>
    {
        public int Add(Seat entity)
        {
            using var conn = DBConnection.GetConnection();
            string query = "INSERT INTO Seats (HallID, SeatRow, SeatNo, SeatType) VALUES (@HallID, @SeatRow, @SeatNo, @SeatType); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@HallID", entity.HallID);
            cmd.Parameters.AddWithValue("@SeatRow", entity.SeatRow);
            cmd.Parameters.AddWithValue("@SeatNo", entity.SeatNo);
            cmd.Parameters.AddWithValue("@SeatType", entity.SeatType);
            conn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Update(Seat entity)
        {
            using var conn = DBConnection.GetConnection();
            string query = "UPDATE Seats SET HallID=@HallID, SeatRow=@SeatRow, SeatNo=@SeatNo, SeatType=@SeatType WHERE SeatID=@SeatID";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@SeatID", entity.SeatID);
            cmd.Parameters.AddWithValue("@HallID", entity.HallID);
            cmd.Parameters.AddWithValue("@SeatRow", entity.SeatRow);
            cmd.Parameters.AddWithValue("@SeatNo", entity.SeatNo);
            cmd.Parameters.AddWithValue("@SeatType", entity.SeatType);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = DBConnection.GetConnection();
            string query = "DELETE FROM Seats WHERE SeatID=@id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public Seat GetById(int id)
        {
            using var conn = DBConnection.GetConnection();
            string query = "SELECT * FROM Seats WHERE SeatID=@id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read()) return MapFromReader(reader);
            return null;
        }

        public List<Seat> GetAll()
        {
            var list = new List<Seat>();
            using var conn = DBConnection.GetConnection();
            string query = "SELECT * FROM Seats";
            using var cmd = new SqlCommand(query, conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(MapFromReader(reader));
            return list;
        }

        public List<Seat> GetByHallId(int hallId)
        {
            var list = new List<Seat>();
            using var conn = DBConnection.GetConnection();
            string query = "SELECT * FROM Seats WHERE HallID=@HallID";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@HallID", hallId);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(MapFromReader(reader));
            return list;
        }

        public void GenerateSeatsForHall(int hallId, int rows, int seatsPerRow)
        {
            using var conn = DBConnection.GetConnection();
            conn.Open();
            for (int r = 0; r < rows; r++)
            {
                char rowChar = (char)('A' + r);
                for (int s = 1; s <= seatsPerRow; s++)
                {
                    string query = "INSERT INTO Seats (HallID, SeatRow, SeatNo, SeatType) VALUES (@HallID, @SeatRow, @SeatNo, 'Regular')";
                    using var cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@HallID", hallId);
                    cmd.Parameters.AddWithValue("@SeatRow", rowChar.ToString());
                    cmd.Parameters.AddWithValue("@SeatNo", s);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private Seat MapFromReader(SqlDataReader reader)
        {
            return new Seat
            {
                SeatID = Convert.ToInt32(reader["SeatID"]),
                HallID = Convert.ToInt32(reader["HallID"]),
                SeatRow = reader["SeatRow"].ToString(),
                SeatNo = Convert.ToInt32(reader["SeatNo"]),
                SeatType = reader["SeatType"].ToString()
            };
        }
    }
}
