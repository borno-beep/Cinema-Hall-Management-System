namespace CinemaHallSystem.Models
{
    /// <summary>
    /// Represents a seat for a specific show and its booking status.
    /// </summary>
    public class ShowSeat
    {
        public int ShowSeatID { get; set; }
        public int ShowID { get; set; }
        public int SeatID { get; set; }
        public string Status { get; set; } = string.Empty;

        // Display-helper properties
        public string SeatRow { get; set; } = string.Empty;
        public int SeatNo { get; set; }
        public string SeatType { get; set; } = string.Empty;

        public ShowSeat() { }
    }
}
