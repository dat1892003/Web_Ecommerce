using SV21T1020123.DomainModels;

namespace SV21T1020123.Web.Models
{
    public class EmployeeSearchResult : PaginationSearchResult
    {
        public required List<Employee> Data { get; set; }
    }
}
