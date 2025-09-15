namespace Daisy11Functions.Database.NewWorld.Tables
{
    public class Product
    {
        public long id { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }
        public long code { get; set; }
        public long quantity { get; set; }
        public long rating { get; set; }
    }
}