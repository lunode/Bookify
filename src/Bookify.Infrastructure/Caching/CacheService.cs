using System.Buffers;
using System.Text.Json;
using Bookify.Application.Abstractions.Caching;
using Microsoft.Extensions.Caching.Distributed;

namespace Bookify.Infrastructure.Caching;

public sealed class CacheServie(
    IDistributedCache cache
) : ICacheService
{
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        byte[]? bytes = await cache.GetAsync(key, cancellationToken);
        return bytes is null ? default : Deserialize<T>(bytes);
    }

    public Task RemoveAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        return cache.RemoveAsync(key);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? timeSpan = null, CancellationToken cancellationToken = default)
    {
        var buffer = Serialize<T>(value);
        return cache.SetAsync(key, buffer, CacheOptions.Create(timeSpan), cancellationToken);
    }

    private static T Deserialize<T>(byte[] bytes)
    {
        return JsonSerializer.Deserialize<T>(bytes)!;
    }
    private static byte[] Serialize<T>(T value)
    {
        var buffer = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(buffer);
        JsonSerializer.Serialize(writer, value);
        return buffer.WrittenSpan.ToArray();
    }
}