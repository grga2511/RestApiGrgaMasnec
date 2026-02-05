using Microsoft.AspNetCore.Identity;

namespace TehnologijaWebaGrgaMasnec.Entities
{
    public class ApplicationUser:IdentityUser
    {
        public string? FullName { get; set; }

        // jedan korisnik moze napravit vise rezervacija
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    }
}
