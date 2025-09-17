using Daisy11Functions.Database.Maintenance.Tables;

namespace Daisy11Functions.Database.Pagination
{
    public class AssetPaginationObject
    {
        public List<Asset>? Data { get; set; }
        public PaginationData? Pagination { get; set; }
    }
}