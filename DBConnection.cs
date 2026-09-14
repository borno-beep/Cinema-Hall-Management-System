using System.Configuration;
using Microsoft.Data.SqlClient;

namespace CinemaHallSystem.DAL
{
    public static class DBConnection
    {
        private static readonly string _connectionString;

        static DBConnection()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["CinemaDB"]?.ConnectionString
                ?? @"Server=localhost;Database=CinemaHallDB;Trusted_Connection=True;TrustServerCertificate=True;";
        }

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}