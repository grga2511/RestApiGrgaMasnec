namespace TehnologijaWebaGrgaMasnec.Entities
{
    public class RoomType
    {

        public int Id { get; set; }
        public string Name { get; set; } = null!; 
        public int Capacity { get; set; }         
        public decimal BasePricePerNight { get; set; }
        public ICollection<Room> Rooms { get; set; } = new List<Room>();

    }
}
