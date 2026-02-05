namespace TehnologijaWebaGrgaMasnec.Entities
{
    public class TimeSlot
    {

        public int Id { get; set; }

        // jedna soba moze imat vise vremena u danu kad je rezervirana
        public int RoomId { get; set; }
        public Room Room { get; set; } = null!;

        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        // 1 : 0..1  (termin može biti rezerviran ili ne)
        public Reservation? Reservation { get; set; }





    }
}
