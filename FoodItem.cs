namespace CinemaHallSystem.Models
{
    /// <summary>
    /// Represents a food or beverage item available for purchase.
    /// </summary>
    public class FoodItem
    {
        public int FoodItemID { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;

        public FoodItem() { }
    }
}
