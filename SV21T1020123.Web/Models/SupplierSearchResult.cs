using SV21T1020123.DomainModels;

namespace SV21T1020123.Web.Models
{
    public class SupplierSearchResult : PaginationSearchResult
    {
        public required List<Supplier> Data { get; set; }
    }
}
