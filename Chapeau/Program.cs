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

        builder.Services.AddSingleton<IMenuItemService, MenuItemService>();
        builder.Services.AddSingleton<IMenuItemRepository, MenuItemRepository>();


        // Add configuration access
        string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

      
       
        var app = builder.Build();



        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}

