namespace eazy
{
    public class FileOptions
    {
        public bool Enabled { get; set; } = true;
        public string Path { get; set; } = "Logs/logs.txt";
        public string Interval { get; set; } = "day";
    }
}