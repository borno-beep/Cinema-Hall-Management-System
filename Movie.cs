namespace CinemaHallSystem.Models
{
    /// <summary>
    /// Represents a movie in the system.
    /// </summary>
    public class Movie
    {
        public int MovieID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public string Language { get; set; } = string.Empty;
        public string AgeRating { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public string Description { get; set; } = string.Empty;

        public Movie() { }
    }
}
