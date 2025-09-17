using System.ComponentModel.DataAnnotations;

namespace Daisy11Functions.Database.Maintenance.Tables
{
    public class Asset
    {
        [Key]
        public int RecordId { get; set; }
        public int? CustomerId { get; set; }
        public int? CustomerDepotId { get; set; }
        public int FleetManagerId { get; set; }
        public string? RegistrationNumber { get; set; }
        public int? MakeId { get; set; }
        public int? ModelId { get; set; }
        public string? Derivative { get; set; }
        public int AssetTypeId { get; set; }
        public string? AssetName { get; set; }
        public int? AssetWeight { get; set; }
    }
}