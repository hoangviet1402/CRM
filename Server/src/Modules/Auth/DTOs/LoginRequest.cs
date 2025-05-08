using System.ComponentModel.DataAnnotations;

namespace AuthModule.DTOs;

public class LoginRequest
{
    public string AccountName { get; set; }

    public bool IsUsePhone { get; set; }

    public string Password { get; set; }
} 