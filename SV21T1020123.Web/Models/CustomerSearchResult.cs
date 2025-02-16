using SV21T1020123.DomainModels;

namespace SV21T1020123.Web.Models
{
    public class CustomerSearchResult : PaginationSearchResult
    {
         public required List<Customer> Data { get; set; }
    }
}
