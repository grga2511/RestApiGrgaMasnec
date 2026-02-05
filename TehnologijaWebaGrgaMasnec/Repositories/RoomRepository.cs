using Microsoft.EntityFrameworkCore;
using TehnologijaWebaGrgaMasnec.Infrastructure;

namespace TehnologijaWebaGrgaMasnec.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly AppDbContext _db;
    public RoomRepository(AppDbContext db) => _db = db;

    public Task<bool> FacilityExistsAsync(int facilityId) // provjera postoji li hotel
        => _db.Facilities.AnyAsync(f => f.Id == facilityId);

    public Task<bool> RoomTypeExistsAsync(int roomTypeId) // provjera postoji li ta vrsta sobe
        => _db.RoomTypes.AnyAsync(rt => rt.Id == roomTypeId);
}

