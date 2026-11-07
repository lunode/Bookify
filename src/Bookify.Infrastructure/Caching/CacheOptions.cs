using Microsoft.Extensions.Caching.Distributed;

namespace Bookify.Infrastructure.Caching;

public sealed class CacheOptions
{
    public static DistributedCacheEntryOptions DefaultExpiration =
        new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1) };

    public static DistributedCacheEntryOptions Create(TimeSpan? expiration)
    {
        return expiration is null ?
            DefaultExpiration :
            new()
            {
                AbsoluteExpirationRelativeToNow = expiration
            };
    }
}