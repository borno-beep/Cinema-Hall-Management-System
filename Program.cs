using CinemaHallSystem.BLL;
using CinemaHallSystem.Forms;

namespace CinemaHallSystem
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Ensure the default admin account exists on first launch
            try
            {
                AuthManager.EnsureAdminExists();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Database connection failed. Please ensure:\n\n" +
                    "1. SQL Server Express is running (service: MSSQL$SQLEXPRESS)\n" +
                    "2. Database 'CinemaHallDB' has been created (run Database\\Schema.sql)\n" +
                    "3. Seed data has been inserted (run Database\\SeedData.sql)\n\n" +
                    "Error: " + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            Application.Run(new LoginForm());
        }
    }
}
