using Daisy11Functions.Database.Tables;

namespace Daisy11Functions.Database
{
    public class PaginationObject
    {
        public List<Customer>? Data { get; set; }
        public PaginationData? Pagination { get; set; }
    }
}