namespace eazy.logger.Logging.Options
{
    public class DatabaseOptions
    {
        public bool Enabled { get; set; } = false;
        public string Name { get; set; }
        public string Table { get; set; }
        public string Instance { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Encrypt { get; set; }
        public string TrustedConnection { get; set; }
        public string TrustServerCertificate { get; set; }
        public string IntegratedSecurity { get; set; }
    }
}