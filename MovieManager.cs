using System;
using System.Collections.Generic;
using System.Linq;
using CinemaHallSystem.DAL;
using CinemaHallSystem.Models;
using CinemaHallSystem.Utilities;

namespace CinemaHallSystem.BLL
{
    public class MovieManager
    {
        private MovieDAL movieDAL = new MovieDAL();

        public List<Movie> GetAllMovies()
        {
            return movieDAL.GetAll();
        }

        public Movie GetMovie(int id)
        {
            return movieDAL.GetById(id);
        }

        public int AddMovie(Movie m)
        {
            if (!Validator.ValidateMovie(m, out string errorMsg))
                throw new Exception(errorMsg);
            
            return movieDAL.Add(m);
        }

        public void UpdateMovie(Movie m)
        {
            if (!Validator.ValidateMovie(m, out string errorMsg))
                throw new Exception(errorMsg);

            movieDAL.Update(m);
        }

        public void DeleteMovie(int id)
        {
            movieDAL.Delete(id);
        }

        public List<Movie> SearchMovies(string keyword)
        {
            return movieDAL.GetAll().Where(m => 
                m.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) || 
                m.Genre.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }
}
