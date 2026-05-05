namespace LIMTIC.Infrastructure.Settings
{
    public class JwtSettings
    {
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpireInMinutes { get; set; }
        public int ExpireInSeconds { get; set; }
    }
}
