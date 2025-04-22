using System;

namespace AuthModule.DTOs;

public class AccountTokenDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime Expires { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedByIp { get; set; }
} 