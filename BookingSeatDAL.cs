using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CinemaHallSystem.Models;

namespace CinemaHallSystem.DAL
{
    public class BookingSeatDAL
    {
        public void Add(int bookingId, int showSeatId, SqlConnection conn, SqlTransaction tran)
        {
            string query = "INSERT INTO BookingSeats (BookingID, ShowSeatID) VALUES (@BookingID, @ShowSeatID)";
            using var cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@BookingID", bookingId);
            cmd.Parameters.AddWithValue("@ShowSeatID", showSeatId);
            cmd.ExecuteNonQuery();
        }

        public List<BookingSeat> GetByBookingId(int bookingId)
        {
            var list = new List<BookingSeat>();
            using var conn = DBConnection.GetConnection();
            string query = "SELECT * FROM BookingSeats WHERE BookingID=@BookingID";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@BookingID", bookingId);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new BookingSeat
                {
                    BookingSeatID = Convert.ToInt32(reader["BookingSeatID"]),
                    BookingID = Convert.ToInt32(reader["BookingID"]),
                    ShowSeatID = Convert.ToInt32(reader["ShowSeatID"])
                });
            }
            return list;
        }
    }
}
