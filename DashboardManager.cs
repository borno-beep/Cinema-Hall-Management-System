using System;
using Microsoft.Data.SqlClient;
using CinemaHallSystem.DAL;

namespace CinemaHallSystem.BLL
{
    public class DashboardManager
    {
        public int GetTodayBookingCount()
        {
            using var conn = DBConnection.GetConnection();
            string query = "SELECT COUNT(*) FROM Bookings WHERE CAST(BookingDate AS DATE) = CAST(GETDATE() AS DATE) AND Status = 'Confirmed'";
            using var cmd = new SqlCommand(query, conn);
            conn.Open();
            return (int)cmd.ExecuteScalar();
        }

        public decimal GetTodayRevenue()
        {
            using var conn = DBConnection.GetConnection();
            string query = "SELECT ISNULL(SUM(TotalAmount), 0) FROM Bookings WHERE CAST(BookingDate AS DATE) = CAST(GETDATE() AS DATE) AND Status = 'Confirmed'";
            using var cmd = new SqlCommand(query, conn);
            conn.Open();
            return (decimal)cmd.ExecuteScalar();
        }

        public int GetActiveMovieCount()
        {
            using var conn = DBConnection.GetConnection();
            string query = "SELECT COUNT(DISTINCT MovieID) FROM Shows WHERE ShowDate >= CAST(GETDATE() AS DATE)";
            using var cmd = new SqlCommand(query, conn);
            conn.Open();
            return (int)cmd.ExecuteScalar();
        }

        public double GetTodayOccupancyRate()
        {
            using var conn = DBConnection.GetConnection();
            string query = @"SELECT 
                             CAST(SUM(CASE WHEN ss.Status = 'Booked' THEN 1 ELSE 0 END) AS FLOAT) / 
                             NULLIF(COUNT(ss.ShowSeatID), 0) * 100
                             FROM Shows s
                             JOIN ShowSeats ss ON s.ShowID = ss.ShowID
                             WHERE s.ShowDate = CAST(GETDATE() AS DATE)";
            using var cmd = new SqlCommand(query, conn);
            conn.Open();
            var result = cmd.ExecuteScalar();
            if (result == DBNull.Value) return 0.0;
            return Convert.ToDouble(result);
        }

        public int GetTotalCustomers()
        {
            using var conn = DBConnection.GetConnection();
            string query = "SELECT COUNT(*) FROM Users WHERE Role = 'Customer'";
            using var cmd = new SqlCommand(query, conn);
            conn.Open();
            return (int)cmd.ExecuteScalar();
        }
    }
}
