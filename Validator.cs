using System;
using System.Text.RegularExpressions;
using CinemaHallSystem.Models;

namespace CinemaHallSystem.Utilities
{
    public static class Validator
    {
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        public static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;
            return Regex.IsMatch(phone, @"^\d{7,15}$");
        }

        public static bool IsNotEmpty(string value, string fieldName, out string errorMsg)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                errorMsg = $"{fieldName} cannot be empty.";
                return false;
            }
            errorMsg = string.Empty;
            return true;
        }

        public static bool IsPositiveNumber(decimal value, string fieldName, out string errorMsg)
        {
            if (value <= 0)
            {
                errorMsg = $"{fieldName} must be greater than zero.";
                return false;
            }
            errorMsg = string.Empty;
            return true;
        }

        public static bool IsPositiveInt(int value, string fieldName, out string errorMsg)
        {
            if (value <= 0)
            {
                errorMsg = $"{fieldName} must be greater than zero.";
                return false;
            }
            errorMsg = string.Empty;
            return true;
        }

        public static bool ValidateMovie(Movie m, out string errorMsg)
        {
            if (!IsNotEmpty(m.Title, "Movie Title", out errorMsg)) return false;
            if (!IsPositiveInt(m.DurationMinutes, "Duration", out errorMsg)) return false;
            return true;
        }

        public static bool ValidateHall(string name, int rows, int cols, out string errorMsg)
        {
            if (!IsNotEmpty(name, "Hall Name", out errorMsg)) return false;
            if (!IsPositiveInt(rows, "Rows", out errorMsg)) return false;
            if (!IsPositiveInt(cols, "Columns", out errorMsg)) return false;
            return true;
        }

        public static bool ValidateShow(Show s, out string errorMsg)
        {
            if (!IsPositiveInt(s.MovieID, "Movie", out errorMsg)) return false;
            if (!IsPositiveInt(s.HallID, "Hall", out errorMsg)) return false;
            if (!IsPositiveNumber(s.TicketPrice, "Ticket Price", out errorMsg)) return false;
            return true;
        }

        public static bool ValidateRegistration(string username, string password, string confirmPassword, string fullName, out string errorMsg)
        {
            if (!IsNotEmpty(username, "Username", out errorMsg)) return false;
            if (!IsNotEmpty(password, "Password", out errorMsg)) return false;
            if (password != confirmPassword)
            {
                errorMsg = "Passwords do not match.";
                return false;
            }
            if (!IsNotEmpty(fullName, "Full Name", out errorMsg)) return false;
            return true;
        }
    }
}
