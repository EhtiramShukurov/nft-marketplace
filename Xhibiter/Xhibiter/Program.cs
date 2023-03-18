using Microsoft.EntityFrameworkCore;
using Xhibiter.DAL;
using Xhibiter.Models;
using Microsoft.AspNetCore.Identity;
using Xhibiter.Hubs;
using Xhibiter.Abstractions.Services;
using Xhibiter.Services;

namespace Xhibiter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("MSSQL"));
            });
            builder.Services.AddIdentity<NFTUser, IdentityRole>(opt =>
            {
                opt.Password.RequireDigit = true;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequiredLength = 5;
                opt.Lockout.AllowedForNewUsers = true;
                opt.SignIn.RequireConfirmedEmail = true;
                opt.User.RequireUniqueEmail = true;
            }).AddDefaultTokenProviders().AddEntityFrameworkStores<AppDbContext>();
            builder.Services.AddScoped<AuctionService>();
            builder.Services.AddScoped<AuctionTimerService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddSignalR();
            var app = builder.Build();
            app.UseStaticFiles();
            app.UseCors(builder =>
            {
                builder.WithOrigins("http://localhost:5002")
                       .AllowAnyHeader()
                       .WithMethods("GET", "POST")
                       .AllowCredentials();
            });
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
          );
            app.MapHub<AuctionHub>("/auctionHub");
            app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            app.Run();
        }
    }
}