namespace CinemaHallSystem.Models
{
    /// <summary>
    /// Represents an order for food items linked to a booking.
    /// </summary>
    public class FoodOrder
    {
        public int FoodOrderID { get; set; }
        public int BookingID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        public FoodOrder() { }
    }
}
