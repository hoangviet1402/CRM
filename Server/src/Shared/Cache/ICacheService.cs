namespace Shared.Cache;

public interface ICacheService
{
    /// <summary>
    /// Lấy giá trị từ cache theo key
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu của value</typeparam>
    /// <param name="key">Key của cache item</param>
    /// <returns>Giá trị nếu tồn tại và chưa hết hạn, null nếu không tồn tại hoặc đã hết hạn</returns>
    T? Get<T>(string key);

    /// <summary>
    /// Thêm hoặc cập nhật giá trị vào cache
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu của value</typeparam>
    /// <param name="key">Key của cache item</param>
    /// <param name="value">Giá trị cần cache</param>
    /// <param name="expirationSeconds">Thời gian hết hạn tính bằng giây, mặc định là 1800 giây (30 phút)</param>
    void Set<T>(string key, T value, int expirationSeconds = 1800);

    /// <summary>
    /// Xóa một item khỏi cache
    /// </summary>
    /// <param name="key">Key của cache item cần xóa</param>
    void Remove(string key);

    /// <summary>
    /// Xóa tất cả các items đã hết hạn khỏi cache
    /// </summary>
    void RemoveExpiredItems();

    /// <summary>
    /// Kiểm tra một key có tồn tại trong cache không
    /// </summary>
    /// <param name="key">Key cần kiểm tra</param>
    /// <returns>true nếu key tồn tại và chưa hết hạn</returns>
    bool Contains(string key);
} 