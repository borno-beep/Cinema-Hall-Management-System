using System.Collections.Generic;
using System.Linq;
using CinemaHallSystem.Models;

namespace CinemaHallSystem.Utilities
{
    public static class PriceCalculator
    {
        public static decimal CalculateTicketTotal(decimal ticketPrice, int seatCount)
        {
            return ticketPrice * seatCount;
        }

        public static decimal CalculateFoodTotal(List<FoodOrderItem> items)
        {
            if (items == null || !items.Any()) return 0;
            return items.Sum(i => i.Subtotal);
        }

        public static decimal CalculateGrandTotal(decimal ticketTotal, decimal foodTotal)
        {
            return ticketTotal + foodTotal;
        }
    }
}
