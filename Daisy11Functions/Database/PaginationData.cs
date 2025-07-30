namespace Daisy11Functions.Database
{
    public class PaginationData
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public bool HasMore { get; set; }
    }
}