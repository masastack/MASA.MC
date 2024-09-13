namespace Masa.Mc.Infrastructure.Cache;

internal class CacheContext : ICacheContext
{
    readonly IMultilevelCacheClient _multilevelCache;

    readonly MultilevelCacheGlobalOptions _cacheOption;

    public CacheContext(
        IMultilevelCacheClient multilevelCache,
        IOptions<MultilevelCacheGlobalOptions> cacheOption)
    {
        _multilevelCache = multilevelCache;
        _cacheOption = cacheOption.Value;
    }


    public async Task SetAsync<T>(string key, T item, CacheEntryOptions? cacheEntryOptions)
    {
        await _multilevelCache.SetAsync(key, item, cacheEntryOptions ?? _cacheOption.CacheEntryOptions);
    }


    public async Task<T?> GetAsync<T>(string key)
    {
        return await _multilevelCache.GetAsync<T>(key);
    }

    public async Task Remove<T>(string key)
    {
       await _multilevelCache.RemoveAsync<T>(key);
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> setter, CacheEntryOptions? cacheEntryOptions)
    {
        cacheEntryOptions ??= _cacheOption.CacheEntryOptions!;

        var value = await _multilevelCache.GetAsync<T>(key);

        if (value != null)
            return value;

        value = await setter();

        await _multilevelCache.SetAsync(key, value, new CombinedCacheEntryOptions
        {
            MemoryCacheEntryOptions = cacheEntryOptions,
            DistributedCacheEntryOptions = cacheEntryOptions
        });

        return value;
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<(T, CacheEntryOptions cacheEntryOptions)>> setter)
    {
        var value = await _multilevelCache.GetAsync<T>(key);

        if (value != null)
            return value;

        var setterResult = await setter();
        value = setterResult.Item1;
        var cacheEntryOptions = setterResult.Item2;

        await _multilevelCache.SetAsync(key, value, new CombinedCacheEntryOptions
        {
            MemoryCacheEntryOptions = cacheEntryOptions,
            DistributedCacheEntryOptions = cacheEntryOptions
        });

        return value;
    }
}
