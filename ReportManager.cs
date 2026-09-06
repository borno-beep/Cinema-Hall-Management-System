using System;
using System.Data;
using Microsoft.Data.SqlClient;
using CinemaHallSystem.DAL;

namespace CinemaHallSystem.BLL
{
    public class ReportManager
    {
        public DataTable GetDailySales(DateTime date)
        {
            using var conn = DBConnection.GetConnection();
            string query = @"SELECT COUNT(*) as BookingCount, ISNULL(SUM(TotalAmount), 0) as TotalRevenue 
                             FROM Bookings WHERE CAST(BookingDate AS DATE) = @Date AND Status = 'Confirmed'";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Date", date.Date);
            using var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        public DataTable GetMonthlyRevenue(int year, int month)
        {
            using var conn = DBConnection.GetConnection();
            string query = @"SELECT CAST(BookingDate AS DATE) as Date, SUM(TotalAmount) as DailyRevenue
                             FROM Bookings 
                             WHERE YEAR(BookingDate) = @Year AND MONTH(BookingDate) = @Month AND Status = 'Confirmed'
                             GROUP BY CAST(BookingDate AS DATE)
                             ORDER BY Date";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Year", year);
            cmd.Parameters.AddWithValue("@Month", month);
            using var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        public DataTable GetMoviePopularity(DateTime? fromDate, DateTime? toDate)
        {
            using var conn = DBConnection.GetConnection();
            string query = @"SELECT m.Title, COUNT(b.BookingID) as TotalBookings
                             FROM Movies m
                             JOIN Shows s ON m.MovieID = s.MovieID
                             JOIN Bookings b ON s.ShowID = b.ShowID
                             WHERE b.Status = 'Confirmed'
                             AND (@FromDate IS NULL OR b.BookingDate >= @FromDate)
                             AND (@ToDate IS NULL OR b.BookingDate <= @ToDate)
                             GROUP BY m.Title
                             ORDER BY TotalBookings DESC";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@FromDate", fromDate.HasValue ? (object)fromDate.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@ToDate", toDate.HasValue ? (object)toDate.Value : DBNull.Value);
            using var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        public DataTable GetHallOccupancy(DateTime? fromDate, DateTime? toDate)
        {
            using var conn = DBConnection.GetConnection();
            string query = @"SELECT h.HallName, 
                             SUM(CASE WHEN ss.Status = 'Booked' THEN 1 ELSE 0 END) as BookedSeats,
                             COUNT(ss.ShowSeatID) as TotalSeats
                             FROM Halls h
                             JOIN Shows s ON h.HallID = s.HallID
                             JOIN ShowSeats ss ON s.ShowID = ss.ShowID
                             WHERE (@FromDate IS NULL OR s.ShowDate >= @FromDate)
                             AND (@ToDate IS NULL OR s.ShowDate <= @ToDate)
                             GROUP BY h.HallName";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@FromDate", fromDate.HasValue ? (object)fromDate.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@ToDate", toDate.HasValue ? (object)toDate.Value : DBNull.Value);
            using var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        public DataTable GetFoodSales(DateTime? fromDate, DateTime? toDate)
        {
            using var conn = DBConnection.GetConnection();
            string query = @"SELECT fi.Name, SUM(foi.Quantity) as TotalQuantity, SUM(foi.Subtotal) as TotalRevenue
                             FROM FoodItems fi
                             JOIN FoodOrderItems foi ON fi.FoodItemID = foi.FoodItemID
                             JOIN FoodOrders fo ON foi.FoodOrderID = fo.FoodOrderID
                             WHERE (@FromDate IS NULL OR fo.OrderDate >= @FromDate)
                             AND (@ToDate IS NULL OR fo.OrderDate <= @ToDate)
                             GROUP BY fi.Name
                             ORDER BY TotalRevenue DESC";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@FromDate", fromDate.HasValue ? (object)fromDate.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@ToDate", toDate.HasValue ? (object)toDate.Value : DBNull.Value);
            using var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        public DataTable GetBookingHistory(DateTime? fromDate, DateTime? toDate)
        {
            using var conn = DBConnection.GetConnection();
            string query = @"SELECT b.BookingID, u.Username, m.Title, s.ShowDate, b.TotalAmount, b.Status
                             FROM Bookings b
                             JOIN Users u ON b.UserID = u.UserID
                             JOIN Shows s ON b.ShowID = s.ShowID
                             JOIN Movies m ON s.MovieID = m.MovieID
                             WHERE (@FromDate IS NULL OR b.BookingDate >= @FromDate)
                             AND (@ToDate IS NULL OR b.BookingDate <= @ToDate)
                             ORDER BY b.BookingDate DESC";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@FromDate", fromDate.HasValue ? (object)fromDate.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@ToDate", toDate.HasValue ? (object)toDate.Value : DBNull.Value);
            using var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        public DataTable GetCancelledBookings(DateTime? fromDate, DateTime? toDate)
        {
            using var conn = DBConnection.GetConnection();
            string query = @"SELECT b.BookingID, u.Username, m.Title, b.TotalAmount, b.BookingDate
                             FROM Bookings b
                             JOIN Users u ON b.UserID = u.UserID
                             JOIN Shows s ON b.ShowID = s.ShowID
                             JOIN Movies m ON s.MovieID = m.MovieID
                             WHERE b.Status = 'Cancelled'
                             AND (@FromDate IS NULL OR b.BookingDate >= @FromDate)
                             AND (@ToDate IS NULL OR b.BookingDate <= @ToDate)
                             ORDER BY b.BookingDate DESC";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@FromDate", fromDate.HasValue ? (object)fromDate.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@ToDate", toDate.HasValue ? (object)toDate.Value : DBNull.Value);
            using var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
    }
}
