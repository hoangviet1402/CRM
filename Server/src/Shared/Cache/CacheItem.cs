namespace Shared.Cache;

public class CacheItem<T>
{
    public T Value { get; set; }
    public DateTime ExpirationTime { get; set; }

    public CacheItem(T value, DateTime expirationTime)
    {
        Value = value;
        ExpirationTime = expirationTime;
    }

    public bool IsExpired => DateTime.UtcNow > ExpirationTime;
} 