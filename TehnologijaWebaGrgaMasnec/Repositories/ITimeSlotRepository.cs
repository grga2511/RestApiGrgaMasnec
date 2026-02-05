using TehnologijaWebaGrgaMasnec.Entities;

namespace TehnologijaWebaGrgaMasnec.Repositories;

public interface ITimeSlotRepository
{
    Task<bool> RoomExistsAsync(int roomId);
    Task<bool> SlotExistsAsync(int timeSlotId);

    Task<bool> HasOverlapAsync(int roomId, DateTime start, DateTime end);

    Task<List<object>> GetAllAsync();
    Task<List<object>> GetAvailableAsync(int roomId, DateTime from, DateTime to);

    Task<TimeSlot> AddAsync(TimeSlot slot);
    Task<int> CountAsync();
}

