namespace TehnologijaWebaGrgaMasnec.Entities
{
    public class Review
    {

        public int Id { get; set; }

        // Jedna rezervacija ima jedan review
        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; } = null!;

        public int Rating { get; set; } 
        public string? Comment { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;

    }
}
