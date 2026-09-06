using System.Collections.Generic;
using CinemaHallSystem.DAL;
using CinemaHallSystem.Models;

namespace CinemaHallSystem.BLL
{
    public class FoodManager
    {
        private FoodItemDAL foodItemDAL = new FoodItemDAL();

        public List<FoodItem> GetAllFoodItems()
        {
            return foodItemDAL.GetAll();
        }

        public int AddFoodItem(FoodItem item)
        {
            return foodItemDAL.Add(item);
        }

        public void UpdateFoodItem(FoodItem item)
        {
            foodItemDAL.Update(item);
        }

        public void DeleteFoodItem(int id)
        {
            foodItemDAL.Delete(id);
        }
    }
}
