using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CinemaHallSystem.Models;

namespace CinemaHallSystem.DAL
{
    public class FoodOrderItemDAL
    {
        public List<FoodOrderItem> GetByFoodOrderId(int foodOrderId)
        {
            var list = new List<FoodOrderItem>();
            using var conn = DBConnection.GetConnection();
            string query = "SELECT i.*, f.Name AS FoodItemName FROM FoodOrderItems i JOIN FoodItems f ON i.FoodItemID = f.FoodItemID WHERE i.FoodOrderID=@FoodOrderID";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@FoodOrderID", foodOrderId);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var item = new FoodOrderItem
                {
                    FoodOrderItemID = Convert.ToInt32(reader["FoodOrderItemID"]),
                    FoodOrderID = Convert.ToInt32(reader["FoodOrderID"]),
                    FoodItemID = Convert.ToInt32(reader["FoodItemID"]),
                    Quantity = Convert.ToInt32(reader["Quantity"]),
                    Subtotal = Convert.ToDecimal(reader["Subtotal"])
                };

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (reader.GetName(i).Equals("FoodItemName", StringComparison.OrdinalIgnoreCase))
                        item.FoodItemName = reader["FoodItemName"].ToString();
                }

                list.Add(item);
            }
            return list;
        }

        public void Add(FoodOrderItem item, SqlConnection conn, SqlTransaction tran)
        {
            string query = "INSERT INTO FoodOrderItems (FoodOrderID, FoodItemID, Quantity, Subtotal) VALUES (@FoodOrderID, @FoodItemID, @Quantity, @Subtotal)";
            using var cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@FoodOrderID", item.FoodOrderID);
            cmd.Parameters.AddWithValue("@FoodItemID", item.FoodItemID);
            cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
            cmd.Parameters.AddWithValue("@Subtotal", item.Subtotal);
            cmd.ExecuteNonQuery();
        }
    }
}
