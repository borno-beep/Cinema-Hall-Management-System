using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CinemaHallSystem.Models;

namespace CinemaHallSystem.DAL
{
    public class PaymentDAL : IRepository<Payment>
    {
        public int Add(Payment entity)
        {
            using var conn = DBConnection.GetConnection();
            string query = "INSERT INTO Payments (BookingID, Amount, PaymentMethod, PaymentDate, Status) VALUES (@BookingID, @Amount, @PaymentMethod, @PaymentDate, @Status); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@BookingID", entity.BookingID);
            cmd.Parameters.AddWithValue("@Amount", entity.Amount);
            cmd.Parameters.AddWithValue("@PaymentMethod", entity.PaymentMethod);
            cmd.Parameters.AddWithValue("@PaymentDate", entity.PaymentDate);
            cmd.Parameters.AddWithValue("@Status", entity.Status);
            conn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Update(Payment entity)
        {
            using var conn = DBConnection.GetConnection();
            string query = "UPDATE Payments SET BookingID=@BookingID, Amount=@Amount, PaymentMethod=@PaymentMethod, PaymentDate=@PaymentDate, Status=@Status WHERE PaymentID=@PaymentID";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@PaymentID", entity.PaymentID);
            cmd.Parameters.AddWithValue("@BookingID", entity.BookingID);
            cmd.Parameters.AddWithValue("@Amount", entity.Amount);
            cmd.Parameters.AddWithValue("@PaymentMethod", entity.PaymentMethod);
            cmd.Parameters.AddWithValue("@PaymentDate", entity.PaymentDate);
            cmd.Parameters.AddWithValue("@Status", entity.Status);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = DBConnection.GetConnection();
            string query = "DELETE FROM Payments WHERE PaymentID=@id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public Payment GetById(int id)
        {
            using var conn = DBConnection.GetConnection();
            string query = "SELECT * FROM Payments WHERE PaymentID=@id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read()) return MapFromReader(reader);
            return null;
        }

        public List<Payment> GetAll()
        {
            var list = new List<Payment>();
            using var conn = DBConnection.GetConnection();
            string query = "SELECT * FROM Payments";
            using var cmd = new SqlCommand(query, conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(MapFromReader(reader));
            return list;
        }

        public Payment GetByBookingId(int bookingId)
        {
            using var conn = DBConnection.GetConnection();
            string query = "SELECT * FROM Payments WHERE BookingID=@BookingID";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@BookingID", bookingId);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read()) return MapFromReader(reader);
            return null;
        }

        public int Add(Payment payment, SqlConnection conn, SqlTransaction tran)
        {
            string query = "INSERT INTO Payments (BookingID, Amount, PaymentMethod, PaymentDate, Status) VALUES (@BookingID, @Amount, @PaymentMethod, @PaymentDate, @Status); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@BookingID", payment.BookingID);
            cmd.Parameters.AddWithValue("@Amount", payment.Amount);
            cmd.Parameters.AddWithValue("@PaymentMethod", payment.PaymentMethod);
            cmd.Parameters.AddWithValue("@PaymentDate", payment.PaymentDate);
            cmd.Parameters.AddWithValue("@Status", payment.Status);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        private Payment MapFromReader(SqlDataReader reader)
        {
            return new Payment
            {
                PaymentID = Convert.ToInt32(reader["PaymentID"]),
                BookingID = Convert.ToInt32(reader["BookingID"]),
                Amount = Convert.ToDecimal(reader["Amount"]),
                PaymentMethod = reader["PaymentMethod"].ToString(),
                PaymentDate = Convert.ToDateTime(reader["PaymentDate"]),
                Status = reader["Status"].ToString()
            };
        }
    }
}
