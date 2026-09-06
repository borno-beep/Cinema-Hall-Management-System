namespace CinemaHallSystem.Models
{
    /// <summary>
    /// Represents a cinema hall.
    /// </summary>
    public class Hall
    {
        public int HallID { get; set; }
        public string HallName { get; set; } = string.Empty;
        public int TotalSeats { get; set; }

        public Hall() { }
    }
}
