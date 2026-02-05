namespace TehnologijaWebaGrgaMasnec.Entities
{
    public class Amenity
    {

        public int Id { get; set; }
        public string Name { get; set; } = null!; // WiFi, Balkon, Klima...

        // vise soba mogu imat vise dodataka
        public ICollection<RoomAmenity> RoomAmenities { get; set; } = new List<RoomAmenity>();

    }
}
