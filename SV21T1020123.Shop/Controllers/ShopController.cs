using Microsoft.AspNetCore.Mvc;
using SV21T1020123.BusinessLayers;
using SV21T1020123.Shop.Models;
using System.Reflection;

namespace SV21T1020123.Shop.Controllers
{
    public class ShopController : Controller
    {
        private const int PAGE_SIZE = 9;
        private const string SHOP_PRODUCT_SEARCH_CONDITION = "ShopProductSearchCondition";
        public IActionResult Index()
        {
            ShopProductSearchInput? condition = ApplicationContext.GetSessionData<ShopProductSearchInput>(SHOP_PRODUCT_SEARCH_CONDITION);
            if (condition == null)
            {
                condition = new ShopProductSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    CategoryID = 0,
                    SupplierID = 0,
                    MaxPrice = 0,
                    MinPrice = 0,
                };
            }
            return View(condition);
        }
        public IActionResult Search(ShopProductSearchInput condition)
        {
            int rowCount;
            var data = ProductDataService.ListProducts(out rowCount, condition.Page, condition.PageSize, condition.SearchValue, condition.CategoryID,
                                                        condition.SupplierID, condition.MinPrice, condition.MaxPrice);
            ShopProductSearchResult model = new ShopProductSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                CategoryID = condition.CategoryID,
                SupplierID = condition.SupplierID,
                MaxPrice = condition.MaxPrice,
                MinPrice = condition.MinPrice,
                Data = data,
                RowCount = rowCount,
            };
            ApplicationContext.SetSessionData(SHOP_PRODUCT_SEARCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult ShopDetail(int id)
        {
            var data = ProductDataService.GetProduct(id);
            return View(data);
        }
    }
}
