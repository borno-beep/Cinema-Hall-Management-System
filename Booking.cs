namespace CinemaHallSystem.Models
{
    /// <summary>
    /// Represents a user's booking for a show.
    /// </summary>
    public class Booking
    {
        public int BookingID { get; set; }
        public int UserID { get; set; }
        public int ShowID { get; set; }
        public DateTime BookingDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;

        // Display-helper properties
        public string MovieTitle { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string HallName { get; set; } = string.Empty;
        public DateTime ShowDate { get; set; }
        public TimeSpan ShowTime { get; set; }

        public Booking() { }
    }
}
