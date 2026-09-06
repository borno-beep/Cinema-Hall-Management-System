namespace CinemaHallSystem.Models
{
    /// <summary>
    /// Represents an individual item within a food order.
    /// </summary>
    public class FoodOrderItem
    {
        public int FoodOrderItemID { get; set; }
        public int FoodOrderID { get; set; }
        public int FoodItemID { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }

        // Display helper
        public string FoodItemName { get; set; } = string.Empty;

        public FoodOrderItem() { }
    }
}
