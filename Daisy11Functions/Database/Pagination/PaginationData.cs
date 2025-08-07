namespace Daisy11Functions.Database.Pagination
{
    public class PaginationData
    {
        public int PageId { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public bool HasMore { get; set; }
    }
}