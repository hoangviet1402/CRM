namespace Shared.Constants;

public static class AppConstants
{
    public const string JWT_SECRET_KEY = "Jwt:SecretKey";
    public const string JWT_ISSUER = "Jwt:Issuer";
    public const string JWT_AUDIENCE = "Jwt:Audience";
    public const int JWT_LIFETIME_MINUTES = 60;
    public const int REFRESH_TOKEN_DAYS = 7;
} 