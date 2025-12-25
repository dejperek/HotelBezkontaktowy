using HotelBezkontaktowy.Data;
using HotelBezkontaktowy.Data.Seed;
using HotelBezkontaktowy.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HotelBezkontaktowy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // DbContext (SQLite)
            builder.Services.AddDbContext<AppDbContext>(opt =>
                opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Identity with cookie authentication
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });

            // Application services
            builder.Services.AddScoped<IRoomService, RoomService>();
            builder.Services.AddScoped<IReservationService, ReservationService>();
            builder.Services.AddScoped<IReportsService, ReportsService>();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Seed roles and admin user
            DbInitializer.InitializeAsync(app.Services).GetAwaiter().GetResult();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
