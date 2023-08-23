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
        var payOrder = await context.PayOrders.Include(order => order.PaidSeats)
            .SingleOrDefaultAsync(payOrder => payOrder.Id == id);
        
        if (payOrder is null) return;

        if (payOrder.Paid) return;
        
        foreach (var seatPaid in payOrder.PaidSeats)
        {
            var seat = await context.AvailableSeats.SingleOrDefaultAsync(seat => seat.Id == seatPaid.Id);
            if (seat is null) return;
            
            seat.Available = true;

            context.AvailableSeats.Update(seat);
            await context.SaveChangesAsync();
        }

        context.PayOrders.Remove(payOrder);
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