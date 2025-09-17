using Daisy11Functions.Database.Maintenance.Tables;

namespace Daisy11Functions.Database.Pagination
{
    public class AccountPaginationObject
    {
        public List<Account>? Data { get; set; }
        public PaginationData? Pagination { get; set; }
    }
}