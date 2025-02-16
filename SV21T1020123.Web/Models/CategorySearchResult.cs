using SV21T1020123.DomainModels;

namespace SV21T1020123.Web.Models
{
    public class CategorySearchResult : PaginationSearchResult
    {
        public required List<Category> Data { get; set; }
    }
}
