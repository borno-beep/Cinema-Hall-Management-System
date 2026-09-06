using System;
using System.Collections.Generic;
using CinemaHallSystem.DAL;
using CinemaHallSystem.Models;
using CinemaHallSystem.Utilities;

namespace CinemaHallSystem.BLL
{
    public class HallManager
    {
        private HallDAL hallDAL = new HallDAL();
        private SeatDAL seatDAL = new SeatDAL();

        public List<Hall> GetAllHalls()
        {
            return hallDAL.GetAll();
        }

        public Hall GetHall(int id)
        {
            return hallDAL.GetById(id);
        }

        public int AddHall(string hallName, int rows, int seatsPerRow)
        {
            if (!Validator.ValidateHall(hallName, rows, seatsPerRow, out string errorMsg))
                throw new Exception(errorMsg);

            var hall = new Hall
            {
                HallName = hallName,
                TotalSeats = rows * seatsPerRow
            };

            int hallId = hallDAL.Add(hall);
            seatDAL.GenerateSeatsForHall(hallId, rows, seatsPerRow);
            return hallId;
        }

        public void DeleteHall(int id)
        {
            hallDAL.Delete(id);
        }
    }
}
