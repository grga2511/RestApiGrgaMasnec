using TehnologijaWebaGrgaMasnec.Entities;

namespace TehnologijaWebaGrgaMasnec.Repositories;

public interface IReservationRepository
{
    Task<bool> IsSlotTakenAsync(int timeSlotId);
    Task<Reservation> AddAsync(Reservation reservation);

    Task<List<object>> GetMineAsync(string userId);

    Task<Reservation?> FindByIdAsync(int reservationId);
    Task SaveChangesAsync();
}

