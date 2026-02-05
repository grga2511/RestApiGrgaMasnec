using Microsoft.EntityFrameworkCore;
using TehnologijaWebaGrgaMasnec.Entities;
using TehnologijaWebaGrgaMasnec.Infrastructure;

namespace TehnologijaWebaGrgaMasnec.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly AppDbContext _db;
    public ReservationRepository(AppDbContext db) => _db = db;

    public Task<bool> IsSlotTakenAsync(int timeSlotId)
        => _db.Reservations.AnyAsync(r =>
            r.TimeSlotId == timeSlotId && r.Status == Reservation.ReservationStatus.Active);

    public async Task<Reservation> AddAsync(Reservation reservation) // dodajemo rezervaciju u bazu
    {
        _db.Reservations.Add(reservation);
        await _db.SaveChangesAsync();
        return reservation;
    }

    public async Task<List<object>> GetMineAsync(string userId) // dohvacamo rezervacije
    {
        var data = await _db.Reservations
            .AsNoTracking()
            .Where(r => r.UserId == userId)
            .Include(r => r.TimeSlot)
            .ThenInclude(t => t.Room)
            .Select(r => new
            {
                r.Id,
                r.Status,
                r.Created,
                TimeSlot = new
                {
                    r.TimeSlotId,
                    r.TimeSlot.Start,
                    r.TimeSlot.End,
                    RoomNumber = r.TimeSlot.Room.Number
                }
            })
            .ToListAsync();

        return data.Cast<object>().ToList();
    }

    public Task<Reservation?> FindByIdAsync(int reservationId) // trazimo rezervaciju po ID-u
        => _db.Reservations.FirstOrDefaultAsync(r => r.Id == reservationId);

    public Task SaveChangesAsync()
        => _db.SaveChangesAsync();
}

