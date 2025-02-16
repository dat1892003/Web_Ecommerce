using SV21T1020123.DomainModels;

namespace SV21T1020123.Shop.Models
{
    public class OrderSearchResult : PaginationSearchResult
    {
        public int Status { get; set; } = 0;
        public string TimeRange { get; set; } = "";
        public int CustomerId { get; set; }
        public required List<Order> data { get; set; }
    }
}
