using System;
using CinemaHallSystem.DAL;
using CinemaHallSystem.Models;
using CinemaHallSystem.Utilities;

namespace CinemaHallSystem.BLL
{
    public class AuthManager
    {
        private static UserDAL userDAL = new UserDAL();
        public static AppUser CurrentUser { get; set; }

        public static AppUser Login(string username, string password)
        {
            var user = userDAL.GetByUsername(username);
            if (user != null && PasswordHasher.VerifyPassword(password, user.PasswordHash))
            {
                CurrentUser = user;
                return user;
            }
            return null;
        }

        public static bool Register(string username, string password, string fullName, string phone, string email)
        {
            if (userDAL.UsernameExists(username))
                throw new Exception("Username already exists.");

            var user = new Customer
            {
                Username = username,
                PasswordHash = PasswordHasher.HashPassword(password),
                FullName = fullName,
                Phone = phone,
                Email = email,
                CreatedDate = DateTime.Now
            };

            return userDAL.Add(user) > 0;
        }

        public static void EnsureAdminExists()
        {
            if (!userDAL.UsernameExists("admin"))
            {
                var admin = new Admin
                {
                    Username = "admin",
                    PasswordHash = PasswordHasher.HashPassword("admin123"),
                    FullName = "System Administrator",
                    Phone = "1234567890",
                    Email = "admin@cinema.com",
                    CreatedDate = DateTime.Now
                };
                userDAL.Add(admin);
            }
        }

        public static void Logout()
        {
            CurrentUser = null;
        }
    }
}
