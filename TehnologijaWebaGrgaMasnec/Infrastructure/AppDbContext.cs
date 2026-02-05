using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using TehnologijaWebaGrgaMasnec.Entities;

namespace TehnologijaWebaGrgaMasnec.Infrastructure;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    
    public DbSet<Facility> Facilities => Set<Facility>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<RoomType> RoomTypes => Set<RoomType>();
    public DbSet<Amenity> Amenities => Set<Amenity>();
    public DbSet<RoomAmenity> RoomAmenities => Set<RoomAmenity>();
    public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Review> Reviews => Set<Review>();


    
    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        
        b.Entity<RoomAmenity>()
            .HasKey(x => new { x.RoomId, x.AmenityId });

        b.Entity<RoomAmenity>()
            .HasOne(x => x.Room)
            .WithMany(r => r.RoomAmenities)
            .HasForeignKey(x => x.RoomId);
            
        b.Entity<RoomAmenity>()
            .HasOne(x => x.Amenity)
            .WithMany(a => a.RoomAmenities)
            .HasForeignKey(x => x.AmenityId);

        
        b.Entity<TimeSlot>()
            .HasOne(t => t.Reservation)
            .WithOne(r => r.TimeSlot)
            .HasForeignKey<Reservation>(r => r.TimeSlotId);

        b.Entity<Reservation>()
            .HasIndex(r => r.TimeSlotId)
            .IsUnique();

        
        b.Entity<Reservation>()
            .HasOne(r => r.Payment)
            .WithOne(p => p.Reservation)
            .HasForeignKey<Payment>(p => p.ReservationId);

        
        b.Entity<Reservation>()
            .HasOne(r => r.Review)
            .WithOne(rv => rv.Reservation)
            .HasForeignKey<Review>(rv => rv.ReservationId);

        b.Entity<Payment>()
            .Property(p => p.Amount)
            .HasPrecision(18, 2);

        b.Entity<RoomType>()
            .Property(rt => rt.BasePricePerNight)
            .HasPrecision(18, 2);

    }
}

