namespace Daisy11Functions.Database.Tables
{
    public class Customer
    {
        public long id { get; set; }
        public string? firstname { get; set; }
        public string? lastname { get; set; }
        public int age { get; set; }
        public bool active { get; set; }
    }
}