using Microsoft.EntityFrameworkCore;
using TehnologijaWebaGrgaMasnec.Entities;

namespace TehnologijaWebaGrgaMasnec.Infrastructure;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // da ne seedamo dvaput
        if (!await db.Facilities.AnyAsync())
        {
            db.Facilities.AddRange(
                new Facility { Name = "Hotel Esplanade", Address = "Zagreb" },
                new Facility { Name = "Hotel Aurora", Address = "Split" }
            );
        }

        if (!await db.RoomTypes.AnyAsync())
        {
            db.RoomTypes.AddRange(
                new RoomType { Name = "Single", Capacity = 1, BasePricePerNight = 70m },
                new RoomType { Name = "Double", Capacity = 2, BasePricePerNight = 110m },
                new RoomType { Name = "Suite", Capacity = 4, BasePricePerNight = 220m }
            );
        }

        await db.SaveChangesAsync();
    }
}

