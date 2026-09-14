using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CinemaHallSystem.Models;

namespace CinemaHallSystem.DAL
{
    public class UserDAL : IRepository<AppUser>
    {
        public int Add(AppUser entity)
        {
            using var conn = DBConnection.GetConnection();
            string query = "INSERT INTO Users (Username, PasswordHash, Role, FullName, Phone, Email, CreatedDate) VALUES (@Username, @PasswordHash, @Role, @FullName, @Phone, @Email, @CreatedDate); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Username", entity.Username);
            cmd.Parameters.AddWithValue("@PasswordHash", entity.PasswordHash);
            cmd.Parameters.AddWithValue("@Role", entity.Role);
            cmd.Parameters.AddWithValue("@FullName", entity.FullName);
            cmd.Parameters.AddWithValue("@Phone", string.IsNullOrEmpty(entity.Phone) ? (object)DBNull.Value : entity.Phone);
            cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(entity.Email) ? (object)DBNull.Value : entity.Email);
            cmd.Parameters.AddWithValue("@CreatedDate", entity.CreatedDate);
            conn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Update(AppUser entity)
        {
            using var conn = DBConnection.GetConnection();
            string query = "UPDATE Users SET Username=@Username, PasswordHash=@PasswordHash, Role=@Role, FullName=@FullName, Phone=@Phone, Email=@Email WHERE UserID=@UserID";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserID", entity.UserID);
            cmd.Parameters.AddWithValue("@Username", entity.Username);
            cmd.Parameters.AddWithValue("@PasswordHash", entity.PasswordHash);
            cmd.Parameters.AddWithValue("@Role", entity.Role);
            cmd.Parameters.AddWithValue("@FullName", entity.FullName);
            cmd.Parameters.AddWithValue("@Phone", string.IsNullOrEmpty(entity.Phone) ? (object)DBNull.Value : entity.Phone);
            cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(entity.Email) ? (object)DBNull.Value : entity.Email);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = DBConnection.GetConnection();
            string query = "DELETE FROM Users WHERE UserID=@id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public AppUser GetById(int id)
        {
            using var conn = DBConnection.GetConnection();
            string query = "SELECT * FROM Users WHERE UserID=@id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return MapFromReader(reader);
            }
            return null;
        }

        public List<AppUser> GetAll()
        {
            var list = new List<AppUser>();
            using var conn = DBConnection.GetConnection();
            string query = "SELECT * FROM Users";
            using var cmd = new SqlCommand(query, conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(MapFromReader(reader));
            }
            return list;
        }

        public AppUser GetByUsername(string username)
        {
            using var conn = DBConnection.GetConnection();
            string query = "SELECT * FROM Users WHERE Username=@Username";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Username", username);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return MapFromReader(reader);
            }
            return null;
        }

        public bool UsernameExists(string username)
        {
            using var conn = DBConnection.GetConnection();
            string query = "SELECT COUNT(1) FROM Users WHERE Username=@Username";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Username", username);
            conn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        private AppUser MapFromReader(SqlDataReader reader)
        {
            string role = reader["Role"].ToString();
            AppUser user = (role == "Admin") ? (AppUser)new Admin() : new Customer();

            user.UserID = Convert.ToInt32(reader["UserID"]);
            user.Username = reader["Username"].ToString();
            user.PasswordHash = reader["PasswordHash"].ToString();
            user.Role = role;
            user.FullName = reader["FullName"].ToString();
            user.Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader["Phone"].ToString();
            user.Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader["Email"].ToString();
            user.CreatedDate = Convert.ToDateTime(reader["CreatedDate"]);

            return user;
        }
    }
}
