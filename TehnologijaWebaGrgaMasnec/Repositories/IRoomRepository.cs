namespace TehnologijaWebaGrgaMasnec.Repositories;

public interface IRoomRepository
{
    Task<bool> FacilityExistsAsync(int facilityId);
    Task<bool> RoomTypeExistsAsync(int roomTypeId);
}

