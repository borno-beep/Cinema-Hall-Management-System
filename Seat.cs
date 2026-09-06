namespace CinemaHallSystem.Models
{
    
    public class Seat
    {
        public int SeatID { get; set; }
        public int HallID { get; set; }
        public string SeatRow { get; set; } = string.Empty;
        public int SeatNo { get; set; }
        public string SeatType { get; set; } = string.Empty;

        public Seat() { }
    }
}
