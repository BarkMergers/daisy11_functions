namespace Daisy11Functions.Database.Archive.Tables
{
    public class Log
    {
        public long id { get; set; }
        public DateTime? timestamp { get; set; }
        public string? message { get; set; }
    }
}