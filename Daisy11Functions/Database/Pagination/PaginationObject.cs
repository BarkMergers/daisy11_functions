using Daisy11Functions.Database.NewWorld.Tables;

namespace Daisy11Functions.Database.Pagination
{
    public class PaginationObject
    {
        public List<Customer>? Data { get; set; }
        public PaginationData? Pagination { get; set; }
    }
}