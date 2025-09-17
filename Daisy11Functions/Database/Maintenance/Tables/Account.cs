using System.ComponentModel.DataAnnotations;

namespace Daisy11Functions.Database.Maintenance.Tables
{
    public class Account
    {

        [Key]
        public int RecordId { get; set; }
        public string? AccountName { get; set; }
        public string? VATRegNo { get; set; }
        public bool OperationalUIAccess { get; set; }
        public string? RegistrationNumber { get; set; }
        public int AccountClassId { get; set; }
        public DateTime? LastUpdateMessageDateTime { get; set; }
        public bool Archived { get; set; }
        public DateTime? ArchivedDateTime { get; set; }
        public bool EnforceTermsAndConditions { get; set; }
    }
}