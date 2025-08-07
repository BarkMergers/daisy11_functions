namespace Daisy11Functions.Database.NewWorld.Tables
{
    public class Agent
    {
        public long id { get; set; }
        public string? agent { get; set; }
        public string? role { get; set; }
        public long tenantid { get; set; }
        public bool active { get; set; }
        public string? firstname { get; set; }
        public string? lastname { get; set; }
        public int? age { get; set; }
    }
}