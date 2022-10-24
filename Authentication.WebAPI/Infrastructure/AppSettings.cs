namespace Authentication.WebAPI.Infrastructure
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public string Iss { get; set; }
        public string Aud { get; set; }
    }
}
