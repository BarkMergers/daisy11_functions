namespace Daisy11Functions.Database.Tables
{
    public class Tenant
    {
        public long id { get; set; }
        public string? tenantname { get; set; }
        public bool active { get; set; }
        public string? subdomain { get; set; }
    }
}