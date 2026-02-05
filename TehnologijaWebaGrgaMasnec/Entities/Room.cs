using System.ComponentModel.Design;

namespace TehnologijaWebaGrgaMasnec.Entities
{
    public class Room
    {
        public int Id { get; set; }

        // jedan hotel ima vise soba
        public int FacilityId { get; set; }
        public Facility Facility { get; set; } = null!;

        // jedna vrsta soba moze imat vise soba
        public int RoomTypeId { get; set; }
        public RoomType RoomType { get; set; } = null!;

        public string Number { get; set; } = null!; 
        public int Floor { get; set; }

        // jedna soba se moze rezervirat u vise razlicitih vremena
        public ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();

        // vise soba mogu imat vise dodataka
        public ICollection<RoomAmenity> RoomAmenities { get; set; } = new List<RoomAmenity>();
    }
}
