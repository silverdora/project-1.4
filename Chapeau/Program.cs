

﻿using Chapeau.Services;
using Chapeau.Models;
using Chapeau.Controllers;
using Chapeau.Repositories.Interfaces;
using Chapeau.Repositories;

using Chapeau.Services.Interfaces;
using Chapeau.Repository.Interface;
using Chapeau.Service.Interface;
using Chapeau.Repository;
using Chapeau.Service;

namespace Chapeau;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        builder.Services.AddScoped<IEmployeeService, EmployeeService>();

        builder.Services.AddScoped<ITableRepository, TableRepository>();
        builder.Services.AddScoped<ITableService, TableService>();

        builder.Services.AddScoped<IDummyOrderRepository, DummyOrderRepository>();
        builder.Services.AddScoped<IDummyOrderService, DummyOrderService>();

        builder.Services.AddScoped<IMenuItemService, MenuItemService>();




        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        // Register application services
        builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
        builder.Services.AddScoped<IPaymentService, PaymentService>();


        builder.Services.AddSingleton<IMenuItemService, MenuItemService>();
        builder.Services.AddSingleton<IMenuItemRepository, MenuItemRepository>();

        builder.Services.AddScoped<IRunningOrdersService, RunningOrdersService>();
        builder.Services.AddScoped<IRunningOrdersRepository, RunningOrdersRepository>();


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

        app.UseSession();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Employee}/{action=Login}/{id?}");

        app.Run();
    }
}

