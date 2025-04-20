using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Web;

namespace Shared.Helpers;

public static class HttpClientHelper
{
    private static readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions _jsonOptions;

    static HttpClientHelper()
    {
        _httpClient = new HttpClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    #region Basic Requests

    /// <summary>
    /// Gửi GET request với custom headers
    /// </summary>
    public static async Task<T?> GetAsync<T>(string url, Dictionary<string, string>? headers = null, string? bearerToken = null)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            AddHeaders(request, headers, bearerToken);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, _jsonOptions);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"GET request failed: {url}", ex);
            throw;
        }
    }

    /// <summary>
    /// Gửi POST request với body và custom headers
    /// </summary>
    public static async Task<TResponse?> PostAsync<TRequest, TResponse>(
        string url, 
        TRequest data, 
        Dictionary<string, string>? headers = null, 
        string? bearerToken = null)
    {
        try
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            AddHeaders(request, headers, bearerToken);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(content, _jsonOptions);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"POST request failed: {url}", ex);
            throw;
        }
    }

    /// <summary>
    /// Gửi PUT request với body và custom headers
    /// </summary>
    public static async Task<TResponse?> PutAsync<TRequest, TResponse>(
        string url, 
        TRequest data, 
        Dictionary<string, string>? headers = null, 
        string? bearerToken = null)
    {
        try
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            using var request = new HttpRequestMessage(HttpMethod.Put, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            AddHeaders(request, headers, bearerToken);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(content, _jsonOptions);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"PUT request failed: {url}", ex);
            throw;
        }
    }

    /// <summary>
    /// Gửi DELETE request với custom headers
    /// </summary>
    public static async Task<bool> DeleteAsync(string url, Dictionary<string, string>? headers = null, string? bearerToken = null)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Delete, url);
            AddHeaders(request, headers, bearerToken);

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"DELETE request failed: {url}", ex);
            throw;
        }
    }

    #endregion

    #region Advanced Requests

    /// <summary>
    /// Gửi GET request với query parameters và custom headers
    /// </summary>
    public static async Task<T?> GetWithQueryAsync<T>(
        string baseUrl, 
        Dictionary<string, string> queryParams, 
        Dictionary<string, string>? headers = null, 
        string? bearerToken = null)
    {
        try
        {
            var query = string.Join("&", queryParams.Select(x => $"{HttpUtility.UrlEncode(x.Key)}={HttpUtility.UrlEncode(x.Value)}"));
            var url = $"{baseUrl}?{query}";

            return await GetAsync<T>(url, headers, bearerToken);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"GET with query request failed: {baseUrl}", ex);
            throw;
        }
    }

    /// <summary>
    /// Gửi POST request với form data và custom headers
    /// </summary>
    public static async Task<T?> PostFormAsync<T>(
        string url, 
        Dictionary<string, string> formData, 
        Dictionary<string, string>? headers = null, 
        string? bearerToken = null)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(formData)
            };

            AddHeaders(request, headers, bearerToken);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, _jsonOptions);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"POST form request failed: {url}", ex);
            throw;
        }
    }

    /// <summary>
    /// Gửi POST request với multipart form data và custom headers
    /// </summary>
    public static async Task<T?> PostMultipartAsync<T>(
        string url, 
        Dictionary<string, string> formData, 
        Dictionary<string, (string fileName, byte[] fileContent, string contentType)> files, 
        Dictionary<string, string>? headers = null, 
        string? bearerToken = null)
    {
        try
        {
            using var multipartContent = new MultipartFormDataContent();

            // Add form fields
            foreach (var field in formData)
            {
                multipartContent.Add(new StringContent(field.Value), field.Key);
            }

            // Add files
            foreach (var file in files)
            {
                var fileContent = new ByteArrayContent(file.Value.fileContent);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.Value.contentType);
                multipartContent.Add(fileContent, file.Key, file.Value.fileName);
            }

            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = multipartContent
            };

            AddHeaders(request, headers, bearerToken);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, _jsonOptions);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"POST multipart request failed: {url}", ex);
            throw;
        }
    }

    /// <summary>
    /// Gửi request với retry logic khi thất bại
    /// </summary>
    public static async Task<T?> SendWithRetryAsync<T>(Func<Task<T?>> requestFunc, int maxRetries = 3, int delayMilliseconds = 1000)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                return await requestFunc();
            }
            catch (Exception ex)
            {
                if (i == maxRetries - 1) throw;
                
                LoggerHelper.Warning($"Request failed, retrying ({i + 1}/{maxRetries}): {ex.Message}");
                await Task.Delay(delayMilliseconds * (i + 1)); // Exponential backoff
            }
        }

        return default;
    }

    /// <summary>
    /// Gửi nhiều requests song song và đợi tất cả hoàn thành
    /// </summary>
    public static async Task<Dictionary<string, T?>> SendParallelAsync<T>(Dictionary<string, Func<Task<T?>>> requests, int maxParallelism = 3)
    {
        var results = new Dictionary<string, T?>();
        var semaphore = new SemaphoreSlim(maxParallelism);
        var tasks = requests.Select(async kvp =>
        {
            await semaphore.WaitAsync();
            try
            {
                results[kvp.Key] = await kvp.Value();
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
        return results;
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Thêm headers vào request
    /// </summary>
    private static void AddHeaders(HttpRequestMessage request, Dictionary<string, string>? headers, string? bearerToken)
    {
        // Add custom headers
        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }

        // Add bearer token if provided
        if (!string.IsNullOrEmpty(bearerToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }
    }

    #endregion

    #region Configuration

    /// <summary>
    /// Cấu hình timeout cho HTTP client
    /// </summary>
    public static void SetTimeout(TimeSpan timeout)
    {
        _httpClient.Timeout = timeout;
    }

    /// <summary>
    /// Thêm header mặc định cho tất cả requests
    /// </summary>
    public static void AddDefaultHeader(string name, string value)
    {
        _httpClient.DefaultRequestHeaders.Add(name, value);
    }

    /// <summary>
    /// Cấu hình base URL cho HTTP client
    /// </summary>
    public static void SetBaseUrl(string baseUrl)
    {
        _httpClient.BaseAddress = new Uri(baseUrl);
    }

    #endregion
} 