namespace CinemaHallSystem.Models
{
    /// <summary>
    /// Represents a payment transaction for a booking.
    /// </summary>
    public class Payment
    {
        public int PaymentID { get; set; }
        public int BookingID { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public string Status { get; set; } = string.Empty;

        public Payment() { }
    }
}
