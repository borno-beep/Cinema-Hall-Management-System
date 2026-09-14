using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CinemaHallSystem.Models;

namespace CinemaHallSystem.DAL
{
    public class FoodOrderDAL
    {
        public int CreateFoodOrder(FoodOrder order, List<FoodOrderItem> items)
        {
            using var conn = DBConnection.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                int id = CreateFoodOrderTransaction(order, items, conn, tran);
                tran.Commit();
                return id;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public int CreateFoodOrderTransaction(FoodOrder order, List<FoodOrderItem> items, SqlConnection conn, SqlTransaction tran)
        {
            string query = "INSERT INTO FoodOrders (BookingID, OrderDate, TotalAmount) VALUES (@BookingID, @OrderDate, @TotalAmount); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@BookingID", order.BookingID);
            cmd.Parameters.AddWithValue("@OrderDate", order.OrderDate);
            cmd.Parameters.AddWithValue("@TotalAmount", order.TotalAmount);
            int id = Convert.ToInt32(cmd.ExecuteScalar());

            var itemDAL = new FoodOrderItemDAL();
            foreach (var item in items)
            {
                item.FoodOrderID = id;
                itemDAL.Add(item, conn, tran);
            }

            return id;
        }

        public List<FoodOrder> GetByBookingId(int bookingId)
        {
            var list = new List<FoodOrder>();
            using var conn = DBConnection.GetConnection();
            string query = "SELECT * FROM FoodOrders WHERE BookingID=@BookingID";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@BookingID", bookingId);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new FoodOrder
                {
                    FoodOrderID = Convert.ToInt32(reader["FoodOrderID"]),
                    BookingID = Convert.ToInt32(reader["BookingID"]),
                    OrderDate = Convert.ToDateTime(reader["OrderDate"]),
                    TotalAmount = Convert.ToDecimal(reader["TotalAmount"])
                });
            }
            return list;
        }
    }
}
