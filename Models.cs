using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }
    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Restaurant>().HasKey(r => r.RestaurantId);
        modelBuilder.Entity<MenuItem>().HasKey(m => m.ItemId);

        modelBuilder.Entity<Restaurant>()
            .Property(r => r.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<MenuItem>()
            .Property(m => m.Price)
            .HasColumnType("decimal(18,2)");

        base.OnModelCreating(modelBuilder);
    }
}

public class Restaurant
{
    public int RestaurantId { get; set; }
    public string Name { get; set; } = null!;
    public string Cuisine { get; set; } = null!;
    public string City { get; set; } = null!;
    public double Rating { get; set; }
    public bool IsOpen { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}

public class MenuItem
{
    [Key]
    public int ItemId { get; set; }
    public int RestaurantId { get; set; }
    public string Name { get; set; } = null!;
    public string Category { get; set; } = null!;
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; } = true;
}
