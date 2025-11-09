using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddControllers().AddJsonOptions(o => { o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "Restaurant Service (SQL Server - Windows Auth)", Version = "v1" }));

var conn = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(conn));

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant Service v1"));
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Could not apply migrations at startup. Ensure SQL Server is running and accessible via Windows Authentication.");
    }

    if (!db.Restaurants.Any())
    {
        db.Restaurants.Add(new Restaurant { Name = "Spice House", Cuisine = "Indian", City = "Srinagar", Rating = 4.5, IsOpen = true });
        db.Restaurants.Add(new Restaurant { Name = "Lake View Cafe", Cuisine = "Cafe", City = "Srinagar", Rating = 4.2, IsOpen = true });
        db.SaveChanges();
        var r = db.Restaurants.First();
        db.MenuItems.Add(new MenuItem { RestaurantId = r.RestaurantId, Name = "Paneer Tikka", Category = "Starter", Price = 150m, IsAvailable = true });
        db.MenuItems.Add(new MenuItem { RestaurantId = r.RestaurantId, Name = "Rogan Josh", Category = "Main", Price = 320m, IsAvailable = true });
        db.SaveChanges();
    }
}

app.Run();
