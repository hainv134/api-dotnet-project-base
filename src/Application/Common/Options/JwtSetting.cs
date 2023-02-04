namespace Application.Common.Options
{
    public class JwtSetting
    {
        public string Secret { get; set; } = String.Empty;
        public string ValidAudience { get; set; } = String.Empty;
        public string ValidIssuer { get; set; } = String.Empty;
        public int TokenLifeTime { get; set; }
    }
}