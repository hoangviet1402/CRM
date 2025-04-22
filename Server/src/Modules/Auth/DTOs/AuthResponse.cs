namespace AuthModule.DTOs;

public class AuthResponse
{
    public int AccountID { get; set; }
    public bool Succeeded { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? ExpiresIn { get; set; }
    public string? Message { get; set; }
    public IEnumerable<string>? Errors { get; set; }
} 