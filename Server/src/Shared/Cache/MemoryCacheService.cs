using System.Collections.Concurrent;

namespace Shared.Cache;

public class MemoryCacheService : ICacheService
{
    private readonly ConcurrentDictionary<string, object> _cache;
    private readonly Timer _cleanupTimer;
    private const int CleanupIntervalSeconds = 300; // 5 phút

    public MemoryCacheService()
    {
        _cache = new ConcurrentDictionary<string, object>();
        
        // Tạo timer để định kỳ dọn dẹp các items hết hạn
        _cleanupTimer = new Timer(CleanupCallback, null, 
            TimeSpan.FromSeconds(CleanupIntervalSeconds), 
            TimeSpan.FromSeconds(CleanupIntervalSeconds));
    }

    public T? Get<T>(string key)
    {
        if (_cache.TryGetValue(key, out var item) && item is CacheItem<T> cacheItem)
        {
            if (!cacheItem.IsExpired)
            {
                return cacheItem.Value;
            }
            
            // Nếu item đã hết hạn, xóa nó
            Remove(key);
        }
        return default;
    }

    public void Set<T>(string key, T value, int expirationSeconds = 1800)
    {
        var expirationTime = DateTime.UtcNow.AddSeconds(expirationSeconds);
        var cacheItem = new CacheItem<T>(value, expirationTime);
        
        _cache.AddOrUpdate(
            key,
            cacheItem,
            (_, _) => cacheItem
        );
    }

    public void Remove(string key)
    {
        _cache.TryRemove(key, out _);
    }

    public void RemoveExpiredItems()
    {
        var expiredKeys = _cache
            .Where(kvp => IsExpired(kvp.Value))
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in expiredKeys)
        {
            Remove(key);
        }
    }

    public bool Contains(string key)
    {
        if (_cache.TryGetValue(key, out var item))
        {
            return !IsExpired(item);
        }
        return false;
    }

    private bool IsExpired(object item)
    {
        // Sử dụng reflection để kiểm tra ExpirationTime
        var expirationProperty = item.GetType().GetProperty("ExpirationTime");
        if (expirationProperty != null)
        {
            var expirationTime = (DateTime)expirationProperty.GetValue(item)!;
            return DateTime.UtcNow > expirationTime;
        }
        return true;
    }

    private void CleanupCallback(object? state)
    {
        RemoveExpiredItems();
    }

    // Implement IDisposable nếu cần
    public void Dispose()
    {
        _cleanupTimer.Dispose();
    }
} 