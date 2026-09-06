namespace CinemaHallSystem.Models
{
    /// <summary>
    /// Links a booking to the specific seats booked.
    /// </summary>
    public class BookingSeat
    {
        public int BookingSeatID { get; set; }
        public int BookingID { get; set; }
        public int ShowSeatID { get; set; }

        public BookingSeat() { }
    }
}
