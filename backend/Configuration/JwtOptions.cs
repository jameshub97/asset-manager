namespace backend.Configuration;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "asset-manager";
    public string Audience { get; set; } = "asset-manager-users";
    public string Secret { get; set; } = "development-only-super-secret-jwt-key-change-me";
    public int ExpirationHours { get; set; } = 1;
}