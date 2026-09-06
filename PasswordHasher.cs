using System;
using System.Security.Cryptography;
using System.Text;

namespace CinemaHallSystem.Utilities
{
    /// <summary>SHA256 + random salt password hashing.</summary>
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            string saltBase64 = Convert.ToBase64String(salt);
            byte[] hashInput = Encoding.UTF8.GetBytes(saltBase64 + password);
            byte[] hash = SHA256.HashData(hashInput);
            return saltBase64 + ":" + Convert.ToBase64String(hash);
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            string[] parts = storedHash.Split(':');
            if (parts.Length != 2) return false;
            string saltBase64 = parts[0];
            byte[] hashInput = Encoding.UTF8.GetBytes(saltBase64 + password);
            byte[] hash = SHA256.HashData(hashInput);
            return Convert.ToBase64String(hash) == parts[1];
        }
    }
}
