using SV21T1020123.DomainModels;

namespace SV21T1020123.Web.Models
{
    public class ShipperSearchResult : PaginationSearchResult
    {
        public required List<Shipper> Data { get; set; }
    }
}
