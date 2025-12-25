using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HotelBezkontaktowy.Models.Entities;

namespace HotelBezkontaktowy.Data
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }

    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<RoomType> RoomTypes => Set<RoomType>();
        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<Reservation> Reservations => Set<Reservation>();
        public DbSet<Amenity> Amenities => Set<Amenity>();
        public DbSet<RoomAmenity> RoomAmenities => Set<RoomAmenity>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<RoomAmenity>().HasKey(ra => new { ra.RoomId, ra.AmenityId });
            builder.Entity<RoomAmenity>()
                .HasOne(ra => ra.Room)
                .WithMany(r => r.RoomAmenities)
                .HasForeignKey(ra => ra.RoomId);
            builder.Entity<RoomAmenity>()
                .HasOne(ra => ra.Amenity)
                .WithMany(a => a.RoomAmenities)
                .HasForeignKey(ra => ra.AmenityId);

            builder.Entity<Room>()
                .HasOne(r => r.RoomType)
                .WithMany(rt => rt.Rooms)
                .HasForeignKey(r => r.RoomTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Reservation>()
                .HasOne(r => r.Room)
                .WithMany(rm => rm.Reservations)
                .HasForeignKey(r => r.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Reservation>()
                .Property(r => r.Status)
                .HasConversion<string>();
        }
    }
}
