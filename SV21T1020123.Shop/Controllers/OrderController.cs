using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020123.BusinessLayers;
using SV21T1020123.Shop.Models;
using System.Globalization;

namespace SV21T1020123.Shop.Controllers
{
    [Authorize(Roles =$"{SV21T1020123.Shop.WebUserRoles.USER}")]
    public class OrderController : Controller
    {
        private const string ORDER_SEARCH_CONDITION = "OrderSearchCondition";
        private const int PAGE_SIZE = 5;
        public IActionResult Index()
        {
            OrderSearchInput? condition = ApplicationContext.GetSessionData<OrderSearchInput>(ORDER_SEARCH_CONDITION);

            if (condition == null)
            {
                condition = new OrderSearchInput()
                {
                    SearchValue = "",
                    Page = 1,
                    CustomerId = int.Parse(User.GetUserData().UserId),
                    PageSize = PAGE_SIZE,
                    Status = 0,
                    TimeRange = $"{DateTime.Today.AddYears(-5).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)} - {DateTime.Today.AddDays(1).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}"
                };
            }
            return View(condition);
        }
        public IActionResult Search(OrderSearchInput conditon)
        {
            int rowCount;
            var data = OrderDataService.CustomerListOrders(out rowCount, conditon.Page, conditon.PageSize, conditon.Status,
                conditon.FromTime, conditon.ToTime, conditon.SearchValue, conditon.CustomerId);
            OrderSearchResult model = new OrderSearchResult()
            {
                data = data,
                TimeRange = conditon.TimeRange,
                CustomerId = conditon.CustomerId,
                RowCount = rowCount,
                Page = conditon.Page,
                PageSize = conditon.PageSize,
                SearchValue = conditon.SearchValue,
                Status = conditon.Status
            };
            ApplicationContext.SetSessionData(ORDER_SEARCH_CONDITION, conditon);
            return View(model);
        }
        public IActionResult Details(int id)
        {
            OrderDetailModel data = new OrderDetailModel()
            {
                Order = SV21T1020123.BusinessLayers.OrderDataService.GetOrder(id),
                Details = SV21T1020123.BusinessLayers.OrderDataService.ListOrderDetails(id),
            };
            return View(data);
        }
    }
}
