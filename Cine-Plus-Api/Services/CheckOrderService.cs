using Cine_Plus_Api.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Microsoft.EntityFrameworkCore;

namespace Cine_Plus_Api.Services;

public class CheckOrderService
{
    private readonly IMemoryCache _cache;

    private readonly IConfiguration _configuration;

    public CheckOrderService(IMemoryCache cache, IConfiguration configuration)
    {
        this._cache = cache;
        this._configuration = configuration;
    }

    public void Add(string key, int obj, TimeSpan expiration)
    {
        var cts = new CancellationTokenSource();

        _cache.Set(key, obj, new MemoryCacheEntryOptions().SetAbsoluteExpiration(expiration)
            .AddExpirationToken(new CancellationChangeToken(cts.Token)));

        var cacheValue = obj;
        _ = Task.Run(async () =>
        {
            await Task.Delay(expiration, cts.Token);

            if (!cts.IsCancellationRequested)
            {
                await Cancel(cacheValue);
            }
        });
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
    }

    private async Task Cancel(int id)
    {
        var context = GetDb();
        var order = await context.Orders.Include(order => order.Seats)
            .SingleOrDefaultAsync(payOrder => payOrder.Id == id);

        if (order is null) return;

        if (order.Paid) return;

        foreach (var seatPaid in order.Seats)
        {
            var seat = await context.Seats.SingleOrDefaultAsync(seat => seat.Id == seatPaid.Id);
            if (seat is null) return;

            seat.State = SeatState.Available;
            seat.Discounts = new List<Discount>();

            context.Seats.Update(seat);
            await context.SaveChangesAsync();
        }

        context.Orders.Remove(order);
        await context.SaveChangesAsync();
    }

    private CinePlusContext GetDb()
    {
        var connectionString = this._configuration.GetConnectionString("DefaultConnection");

        var serverVersion = new MySqlServerVersion(new Version(8, 0, 33));

        return new CinePlusContext(new DbContextOptionsBuilder<CinePlusContext>()
            .UseMySql(connectionString!, serverVersion)
            .Options);
    }
}