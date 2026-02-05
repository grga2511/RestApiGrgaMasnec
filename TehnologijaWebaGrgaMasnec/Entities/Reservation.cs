namespace TehnologijaWebaGrgaMasnec.Entities
{
    public class Reservation
    {
        public enum ReservationStatus
        {
            Active = 1,
            Cancelled = 2,
            Completed = 3
        }


        public int Id { get; set; }

        // Jedan korisnik moze imat vise rezervacija
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        // jedan termin ima max jednu rezervaciju
        public int TimeSlotId { get; set; }
        public TimeSlot TimeSlot { get; set; } = null!;

        public ReservationStatus Status { get; set; } = ReservationStatus.Active;
        public DateTime Created { get; set; } = DateTime.Now;

        
        public Payment? Payment { get; set; }
        public Review? Review { get; set; }

    }
}
