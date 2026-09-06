using CinemaHallSystem.Forms;

namespace CinemaHallSystem.Models
{
    /// <summary>
    /// Abstract base class for all application users.
    /// Demonstrates OOP inheritance — each role overrides GetDashboardForm().
    /// </summary>
    public abstract class AppUser
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Polymorphic method: returns the role-specific dashboard form.
        /// Admin → AdminDashboardForm, Staff → StaffDashboardForm, Customer → CustomerHomeForm.
        /// </summary>
        public abstract Form GetDashboardForm();
    }

    public class Admin : AppUser
    {
        public Admin() { Role = "Admin"; }
        public override Form GetDashboardForm() => new AdminDashboardForm(this);
    }

    public class Customer : AppUser
    {
        public Customer() { Role = "Customer"; }
        public override Form GetDashboardForm() => new CustomerHomeForm(this);
    }
}
