namespace TehnologijaWebaGrgaMasnec.Entities
{
    public class Facility
    {

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Address { get; set; }

        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
