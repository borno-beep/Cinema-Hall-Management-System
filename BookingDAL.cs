using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CinemaHallSystem.Models;

namespace CinemaHallSystem.DAL
{
    public class BookingDAL : IRepository<Booking>
    {
        public int Add(Booking entity)
        {
            using var conn = DBConnection.GetConnection();
            string query = "INSERT INTO Bookings (UserID, ShowID, BookingDate, TotalAmount, Status) VALUES (@UserID, @ShowID, @BookingDate, @TotalAmount, @Status); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserID", entity.UserID);
            cmd.Parameters.AddWithValue("@ShowID", entity.ShowID);
            cmd.Parameters.AddWithValue("@BookingDate", entity.BookingDate);
            cmd.Parameters.AddWithValue("@TotalAmount", entity.TotalAmount);
            cmd.Parameters.AddWithValue("@Status", entity.Status);
            conn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Update(Booking entity)
        {
            using var conn = DBConnection.GetConnection();
            string query = "UPDATE Bookings SET UserID=@UserID, ShowID=@ShowID, BookingDate=@BookingDate, TotalAmount=@TotalAmount, Status=@Status WHERE BookingID=@BookingID";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@BookingID", entity.BookingID);
            cmd.Parameters.AddWithValue("@UserID", entity.UserID);
            cmd.Parameters.AddWithValue("@ShowID", entity.ShowID);
            cmd.Parameters.AddWithValue("@BookingDate", entity.BookingDate);
            cmd.Parameters.AddWithValue("@TotalAmount", entity.TotalAmount);
            cmd.Parameters.AddWithValue("@Status", entity.Status);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = DBConnection.GetConnection();
            string query = "DELETE FROM Bookings WHERE BookingID=@id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public Booking GetById(int id)
        {
            using var conn = DBConnection.GetConnection();
            string query = "SELECT b.*, u.Username AS UserName, s.ShowDate, s.ShowTime, m.Title AS MovieTitle, h.HallName FROM Bookings b JOIN Users u ON b.UserID = u.UserID JOIN Shows s ON b.ShowID = s.ShowID JOIN Movies m ON s.MovieID = m.MovieID JOIN Halls h ON s.HallID = h.HallID WHERE b.BookingID=@id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read()) return MapFromReader(reader);
            return null;
        }

        public List<Booking> GetAll()
        {
            var list = new List<Booking>();
            using var conn = DBConnection.GetConnection();
            string query = "SELECT b.*, u.Username AS UserName, s.ShowDate, s.ShowTime, m.Title AS MovieTitle, h.HallName FROM Bookings b JOIN Users u ON b.UserID = u.UserID JOIN Shows s ON b.ShowID = s.ShowID JOIN Movies m ON s.MovieID = m.MovieID JOIN Halls h ON s.HallID = h.HallID";
            using var cmd = new SqlCommand(query, conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(MapFromReader(reader));
            return list;
        }

        public List<Booking> GetByUserId(int userId)
        {
            var list = new List<Booking>();
            using var conn = DBConnection.GetConnection();
            string query = "SELECT b.*, u.Username AS UserName, s.ShowDate, s.ShowTime, m.Title AS MovieTitle, h.HallName FROM Bookings b JOIN Users u ON b.UserID = u.UserID JOIN Shows s ON b.ShowID = s.ShowID JOIN Movies m ON s.MovieID = m.MovieID JOIN Halls h ON s.HallID = h.HallID WHERE b.UserID=@UserID";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserID", userId);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(MapFromReader(reader));
            return list;
        }

        public int CreateBookingTransaction(Booking booking, List<int> showSeatIds, Payment payment, SqlConnection conn, SqlTransaction tran)
        {
            string bQuery = "INSERT INTO Bookings (UserID, ShowID, BookingDate, TotalAmount, Status) VALUES (@UserID, @ShowID, @BookingDate, @TotalAmount, @Status); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var bCmd = new SqlCommand(bQuery, conn, tran);
            bCmd.Parameters.AddWithValue("@UserID", booking.UserID);
            bCmd.Parameters.AddWithValue("@ShowID", booking.ShowID);
            bCmd.Parameters.AddWithValue("@BookingDate", booking.BookingDate);
            bCmd.Parameters.AddWithValue("@TotalAmount", booking.TotalAmount);
            bCmd.Parameters.AddWithValue("@Status", booking.Status);
            int bookingId = Convert.ToInt32(bCmd.ExecuteScalar());

            var bSeatDAL = new BookingSeatDAL();
            foreach (int showSeatId in showSeatIds)
            {
                bSeatDAL.Add(bookingId, showSeatId, conn, tran);
                string usQuery = "UPDATE ShowSeats SET Status='Booked' WHERE ShowSeatID=@ssid";
                using var usCmd = new SqlCommand(usQuery, conn, tran);
                usCmd.Parameters.AddWithValue("@ssid", showSeatId);
                usCmd.ExecuteNonQuery();
            }

            if (payment != null)
            {
                payment.BookingID = bookingId;
                var paymentDAL = new PaymentDAL();
                paymentDAL.Add(payment, conn, tran);
            }

            return bookingId;
        }

        private Booking MapFromReader(SqlDataReader reader)
        {
            var b = new Booking
            {
                BookingID = Convert.ToInt32(reader["BookingID"]),
                UserID = Convert.ToInt32(reader["UserID"]),
                ShowID = Convert.ToInt32(reader["ShowID"]),
                BookingDate = Convert.ToDateTime(reader["BookingDate"]),
                TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                Status = reader["Status"].ToString()
            };

            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals("UserName", StringComparison.OrdinalIgnoreCase))
                    b.CustomerName = reader["UserName"].ToString();
                if (reader.GetName(i).Equals("MovieTitle", StringComparison.OrdinalIgnoreCase))
                    b.MovieTitle = reader["MovieTitle"].ToString();
                if (reader.GetName(i).Equals("HallName", StringComparison.OrdinalIgnoreCase))
                    b.HallName = reader["HallName"].ToString();
                if (reader.GetName(i).Equals("ShowDate", StringComparison.OrdinalIgnoreCase))
                    b.ShowDate = Convert.ToDateTime(reader["ShowDate"]);
                if (reader.GetName(i).Equals("ShowTime", StringComparison.OrdinalIgnoreCase))
                    b.ShowTime = (TimeSpan)reader["ShowTime"];
            }
            return b;
        }
    }
}
