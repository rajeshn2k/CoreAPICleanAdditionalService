namespace Core.Library.Clean.AdditionalService
{
    /// <summary>
    /// Cache configuration settings
    /// </summary>
    public class CacheSettings
    {
        public string ConnectionString { get; set; } = "localhost:6379";
        public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(30);
        public TimeSpan BookExpiration { get; set; } = TimeSpan.FromMinutes(60);
        public TimeSpan PersonExpiration { get; set; } = TimeSpan.FromMinutes(60);
        public bool EnableCache { get; set; } = true;
    }
}