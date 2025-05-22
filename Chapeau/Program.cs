using Chapeau.Services;
using Chapeau.Models;
using Chapeau.Controllers;
using Chapeau.Repositories.Interfaces;
using Chapeau.Repositories;

namespace Chapeau;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        builder.Services.AddSingleton<IMenuItemService, MenuItemService>();
        builder.Services.AddSingleton<IMenuItemRepository, MenuItemRepository>();

        // Add configuration access
        string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseSession();

        app.UseRouting();

        app.UseAuthorization();

        // Changed the default route to Home/Index
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
