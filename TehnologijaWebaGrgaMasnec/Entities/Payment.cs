namespace TehnologijaWebaGrgaMasnec.Entities
{
    public class Payment
    {

        public int Id { get; set; }

        // 1:1 Reservation -> Payment
        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; } = null!;

        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EUR";
        public string Provider { get; set; } = "Cash"; 
        public DateTime Paid { get; set; } = DateTime.Now;

    }
}
