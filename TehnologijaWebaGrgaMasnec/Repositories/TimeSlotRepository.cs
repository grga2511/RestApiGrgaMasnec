using Microsoft.EntityFrameworkCore;
using TehnologijaWebaGrgaMasnec.Entities;
using TehnologijaWebaGrgaMasnec.Infrastructure;

namespace TehnologijaWebaGrgaMasnec.Repositories;

public class TimeSlotRepository : ITimeSlotRepository
{
    private readonly AppDbContext _db;
    public TimeSlotRepository(AppDbContext db) => _db = db;

    public Task<bool> RoomExistsAsync(int roomId)  //provjerava postoji li ta soba
        => _db.Rooms.AnyAsync(r => r.Id == roomId);

    public Task<bool> SlotExistsAsync(int timeSlotId) // provjerava postoji li taj termin
        => _db.TimeSlots.AnyAsync(t => t.Id == timeSlotId);

    public Task<bool> HasOverlapAsync(int roomId, DateTime start, DateTime end) // provjerava ima li kolizije s terminima
        => _db.TimeSlots.AnyAsync(t =>
            t.RoomId == roomId &&
            start < t.End &&
            end > t.Start);

    public async Task<TimeSlot> AddAsync(TimeSlot slot) // dodavanje termina
    {
        _db.TimeSlots.Add(slot);
        await _db.SaveChangesAsync();
        return slot;
    }

    public Task<int> CountAsync() // broj odnosno kolicina termina
        => _db.TimeSlots.CountAsync();

    public async Task<List<object>> GetAvailableAsync(int roomId, DateTime from, DateTime to) // koliko ima slobodnih termina 
    {
        var data = await _db.TimeSlots
            .AsNoTracking()
            .Where(t => t.RoomId == roomId && t.Start >= from && t.End <= to)
            .Where(t => !_db.Reservations.Any(r =>
                r.TimeSlotId == t.Id && r.Status == Reservation.ReservationStatus.Active))
            .OrderBy(t => t.Start)
            .Select(t => new
            {
                t.Id,
                t.RoomId,
                RoomNumber = t.Room.Number,
                t.Start,
                t.End
            })
            .ToListAsync();

        return data.Cast<object>().ToList();
    }

    public async Task<List<object>> GetAllAsync() // dohvati sve termine 
    {
        var data = await _db.TimeSlots
            .AsNoTracking()
            .Include(t => t.Room)
            .OrderBy(t => t.Start)
            .Select(t => new
            {
                t.Id,
                RoomId = t.RoomId,
                RoomNumber = t.Room.Number,
                t.Start,
                t.End,
                HasReservation = _db.Reservations.Any(r =>
                    r.TimeSlotId == t.Id && r.Status == Reservation.ReservationStatus.Active)
            })
            .ToListAsync();

        return data.Cast<object>().ToList();
    }
}

