using HotelBezkontaktowy.Data;
using HotelBezkontaktowy.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HotelBezkontaktowy.Data.Seed
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var provider = scope.ServiceProvider;

            var db = provider.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync();

            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = new[] { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = "admin@hotel.local";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, "Admin123!@#");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            if (!await db.RoomTypes.AnyAsync())
            {
                var std = new RoomType { Name = "Standard" };
                var dlx = new RoomType { Name = "Deluxe" };
                db.RoomTypes.AddRange(std, dlx);
                await db.SaveChangesAsync();

                db.Rooms.AddRange(
                    new Room { RoomNumber = "101", RoomTypeId = std.Id, Capacity = 2, PricePerNight = 200 },
                    new Room { RoomNumber = "102", RoomTypeId = std.Id, Capacity = 3, PricePerNight = 250 },
                    new Room { RoomNumber = "201", RoomTypeId = dlx.Id, Capacity = 2, PricePerNight = 400 }
                );
                await db.SaveChangesAsync();
            }
        }
    }
}
