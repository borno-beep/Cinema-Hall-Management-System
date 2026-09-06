namespace CinemaHallSystem.Models
{
    /// <summary>
    /// Represents a scheduled movie show.
    /// </summary>
    public class Show
    {
        public int ShowID { get; set; }
        public int MovieID { get; set; }
        public int HallID { get; set; }
        public DateTime ShowDate { get; set; }
        public TimeSpan ShowTime { get; set; }
        public decimal TicketPrice { get; set; }

        // Display-helper properties
        public string MovieTitle { get; set; } = string.Empty;
        public string HallName { get; set; } = string.Empty;

        public Show() { }
    }
}
