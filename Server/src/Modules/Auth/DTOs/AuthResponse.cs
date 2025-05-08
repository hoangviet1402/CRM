using System.Numerics;

namespace AuthModule.DTOs;

public class AuthResponse
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }

    public AuthUserResponse User { get; set; }
    public AuthCompanyResponse Company { get; set; }
}

public class AuthUserResponse
{
    public int? id { get; set; }
    public string fullname { get; set; }    
    public string phone { get; set; }
    public string email { get; set; }
    public int role { get; set; }
    public int need_set_password { get; set; }
    public int is_new_user { get; set; }
    public bool is_active { get; set; }
}

public class AuthCompanyResponse
{
    public int? id { get; set; }
    public string? name { get; set; }
    public int is_active { get; set; }
}