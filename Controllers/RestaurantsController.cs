using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("/v1/restaurants")]
public class RestaurantsController : ControllerBase
{
    private readonly AppDbContext _db;
    public RestaurantsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string? city = null, [FromQuery] string? cuisine = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var q = _db.Restaurants.AsQueryable();
        if (!string.IsNullOrEmpty(city)) q = q.Where(r => r.City == city);
        if (!string.IsNullOrEmpty(cuisine)) q = q.Where(r => r.Cuisine == cuisine);
        var total = await q.CountAsync();
        var items = await q.OrderByDescending(r => r.Rating).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return Ok(new { total, page, pageSize, items });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var r = await _db.Restaurants.FindAsync(id);
        if (r == null) return NotFound();
        return Ok(r);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Restaurant input)
    {
        _db.Restaurants.Add(input);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = input.RestaurantId }, input);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Restaurant input)
    {
        var existing = await _db.Restaurants.FindAsync(id);
        if (existing == null) return NotFound();
        existing.Name = input.Name;
        existing.Cuisine = input.Cuisine;
        existing.City = input.City;
        existing.Rating = input.Rating;
        existing.IsOpen = input.IsOpen;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{id}/menu")]
    public async Task<IActionResult> Menu(int id)
    {
        var items = await _db.MenuItems.Where(m => m.RestaurantId == id).ToListAsync();
        return Ok(items);
    }

    [HttpPost("{id}/menu")]
    public async Task<IActionResult> AddMenu(int id, MenuItem item)
    {
        var rest = await _db.Restaurants.FindAsync(id);
        if (rest == null) return NotFound(new { message = "Restaurant not found" });
        item.RestaurantId = id;
        _db.MenuItems.Add(item);
        await _db.SaveChangesAsync();
        return Ok(item);
    }

    [HttpPut("{restaurantId}/menu/{itemId}")]
    public async Task<IActionResult> UpdateMenu(int restaurantId, int itemId, MenuItem input)
    {
        var item = await _db.MenuItems.FirstOrDefaultAsync(m => m.ItemId == itemId && m.RestaurantId == restaurantId);
        if (item == null) return NotFound();
        item.Name = input.Name;
        item.Category = input.Category;
        item.Price = input.Price;
        item.IsAvailable = input.IsAvailable;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{id}/menu/{itemId}/availability")]
    public async Task<IActionResult> CheckAvailability(int id, int itemId)
    {
        var item = await _db.MenuItems.FirstOrDefaultAsync(m => m.ItemId == itemId && m.RestaurantId == id);
        if (item == null) return NotFound();
        return Ok(new { itemId = item.ItemId, name = item.Name, isAvailable = item.IsAvailable, price = item.Price });
    }
}
