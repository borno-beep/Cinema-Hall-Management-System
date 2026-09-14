using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CinemaHallSystem.Models;

namespace CinemaHallSystem.DAL
{
    public class FoodItemDAL : IRepository<FoodItem>
    {
        public int Add(FoodItem entity)
        {
            using var conn = DBConnection.GetConnection();
            string query = "INSERT INTO FoodItems (Name, Price, Category) VALUES (@Name, @Price, @Category); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Name", entity.Name);
            cmd.Parameters.AddWithValue("@Price", entity.Price);
            cmd.Parameters.AddWithValue("@Category", entity.Category);
            conn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Update(FoodItem entity)
        {
            using var conn = DBConnection.GetConnection();
            string query = "UPDATE FoodItems SET Name=@Name, Price=@Price, Category=@Category WHERE FoodItemID=@FoodItemID";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@FoodItemID", entity.FoodItemID);
            cmd.Parameters.AddWithValue("@Name", entity.Name);
            cmd.Parameters.AddWithValue("@Price", entity.Price);
            cmd.Parameters.AddWithValue("@Category", entity.Category);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = DBConnection.GetConnection();
            string query = "DELETE FROM FoodItems WHERE FoodItemID=@id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public FoodItem GetById(int id)
        {
            using var conn = DBConnection.GetConnection();
            string query = "SELECT * FROM FoodItems WHERE FoodItemID=@id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read()) return MapFromReader(reader);
            return null;
        }

        public List<FoodItem> GetAll()
        {
            var list = new List<FoodItem>();
            using var conn = DBConnection.GetConnection();
            string query = "SELECT * FROM FoodItems";
            using var cmd = new SqlCommand(query, conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(MapFromReader(reader));
            return list;
        }

        public List<FoodItem> GetByCategory(string category)
        {
            var list = new List<FoodItem>();
            using var conn = DBConnection.GetConnection();
            string query = "SELECT * FROM FoodItems WHERE Category=@Category";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Category", category);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(MapFromReader(reader));
            return list;
        }

        private FoodItem MapFromReader(SqlDataReader reader)
        {
            return new FoodItem
            {
                FoodItemID = Convert.ToInt32(reader["FoodItemID"]),
                Name = reader["Name"].ToString(),
                Price = Convert.ToDecimal(reader["Price"]),
                Category = reader["Category"].ToString()
            };
        }
    }
}
