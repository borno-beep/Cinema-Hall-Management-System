using System;
using System.Collections.Generic;
using System.Linq;
using CinemaHallSystem.DAL;
using CinemaHallSystem.Models;
using CinemaHallSystem.Utilities;

namespace CinemaHallSystem.BLL
{
    public class ShowManager
    {
        private ShowDAL showDAL = new ShowDAL();
        private ShowSeatDAL showSeatDAL = new ShowSeatDAL();
        private MovieDAL movieDAL = new MovieDAL();

        public List<Show> GetAllShows()
        {
            return showDAL.GetAll();
        }

        public List<Show> GetUpcomingShows()
        {
            return showDAL.GetAll().Where(s => 
                s.ShowDate > DateTime.Today || 
                (s.ShowDate == DateTime.Today && s.ShowTime > DateTime.Now.TimeOfDay)).ToList();
        }

        public List<Show> GetShowsByMovie(int movieId)
        {
            return showDAL.GetAll().Where(s => s.MovieID == movieId).ToList();
        }

        public List<Show> GetShowsByDate(DateTime date)
        {
            return showDAL.GetAll().Where(s => s.ShowDate.Date == date.Date).ToList();
        }

        public int AddShow(Show s)
        {
            if (!Validator.ValidateShow(s, out string errorMsg))
                throw new Exception(errorMsg);

            var movie = movieDAL.GetById(s.MovieID);
            if (movie == null) throw new Exception("Movie not found.");

            if (showDAL.HasConflict(s.HallID, s.ShowDate, s.ShowTime, movie.DurationMinutes))
                throw new Exception("Show conflicts with an existing show in the same hall.");

            int showId = showDAL.Add(s);
            showSeatDAL.GenerateShowSeats(showId, s.HallID);
            return showId;
        }

        public void DeleteShow(int id)
        {
            showDAL.Delete(id);
        }
    }
}
