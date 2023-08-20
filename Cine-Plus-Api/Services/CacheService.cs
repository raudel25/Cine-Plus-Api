using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Cine_Plus_Api.Services;

public class CacheService
{
    private readonly IMemoryCache _cache;

    public CacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public void Add(string key, int obj, TimeSpan expiration, Func<int, Task> expiredCallback)
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
                await expiredCallback(cacheValue);
            }
        });
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
    }
}