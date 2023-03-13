using ApiAppDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiAppDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<DevicesContext>(opt => opt.UseInMemoryDatabase("HomeAutomationDb"));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.MapGet("/GetDevices", async (DevicesContext Db) => await Db.Devices.ToListAsync());

            app.MapGet("/GetOnDevices", async (DevicesContext Db) => await Db.Devices.Where(t => t.IsDeviceOn).ToListAsync());

            app.MapGet("/GetDevice/{id}", async (int id, DevicesContext Db) =>
            await Db.Devices.FindAsync(id)
                is Device device
                    ? Results.Ok(device)
                    : Results.NotFound());

            app.MapPost("/AddDevices", async (Device device, DevicesContext db) =>
            {
                db.Devices.Add(device);
                await db.SaveChangesAsync();

                return Results.Created($"/AddDevices/{device.DeviceID}", device);
            });

            app.MapPut("/ChangeDevice/{id}", async (int id, Device inputTodo, DevicesContext db) =>
            {
                var todo = await db.Devices.FindAsync(id);

                if (todo is null) return Results.NotFound();

                todo.DeviceName = inputTodo.DeviceName;
                todo.IsDeviceOn = inputTodo.IsDeviceOn;

                await db.SaveChangesAsync();

                return Results.NoContent();
            });

            app.MapDelete("/DeleteDevice/{id}", async (int id, DevicesContext db) =>
            {
                if (await db.Devices.FindAsync(id) is Device device)
                {
                    db.Devices.Remove(device);
                    await db.SaveChangesAsync();
                    return Results.Ok(device);
                }

                return Results.NotFound();
            });

            app.Run();
        }
    }
}