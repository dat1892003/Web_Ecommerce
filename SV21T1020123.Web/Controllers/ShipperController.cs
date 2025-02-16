using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020123.BusinessLayers;
using SV21T1020123.DomainModels;
using SV21T1020123.Web.AppCodes;
using SV21T1020123.Web.Models;

namespace SV21T1020123.Web.Controllers
{
    [Authorize]
    public class ShipperController : Controller
    {
        private const int PAGE_SIZE = 10;
        private const string SHIPPER_SEARCH_CONDITION = "ShipperSearchCondition";
        public IActionResult Index()
        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(SHIPPER_SEARCH_CONDITION);
            if (condition == null)
                condition = new PaginationSearchInput()
                {
                    Page =1,
                    PageSize = PAGE_SIZE,
                    SearchValue =""
                };
            return View(condition);
        }
        public IActionResult Search(PaginationSearchInput condition)
        {
            int rowCount;
            var data = CommonDataService.ListOfShippers(out rowCount,condition.Page,condition.PageSize,condition.SearchValue);
            ShipperSearchResult model = new ShipperSearchResult()
            {
                Data = data,
                RowCount = rowCount,
                Page = condition.Page,
                SearchValue = condition.SearchValue ?? "",
                PageSize =condition.PageSize
            };
            ApplicationContext.SetSessionData(SHIPPER_SEARCH_CONDITION,condition);
            return View(model);
        }
        public IActionResult Create()
        {
            @ViewBag.Title = "Bổ sung người giao hàng mới";
            Shipper data = new Shipper()
            {
                ShipperID = 0,
            };
            return View("Edit", data);
        }
        public IActionResult Edit(int id=0)
        {
            @ViewBag.Title = "Cập nhật thông tin người giao hàng";
            var data = CommonDataService.GetShipper(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }
        [HttpPost]
        public IActionResult Save(Shipper data)
        {
            //TODO: Kiểm soát dữ liệu đầu vào
            ViewBag.Title = data.ShipperID == 0 ? "Bổ sung người giao hàng mới" : "Cập nhật thông tin người giao hàng";
            //Kiểm tra dữ liệu nếu không hợp lệ tạo ra một thông báo lỗi lưu thông báo lỗi vào ModelState
            if (string.IsNullOrWhiteSpace(data.ShipperName))
                ModelState.AddModelError(nameof(data.ShipperName), "Tên người giao hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Số điện thoại không được để trống");
            if (!ModelState.IsValid)
            {
                return View("Edit",data);
            }
            try
            {
                if (data.ShipperID == 0)
                {
                    int id = CommonDataService.AddShipper(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.Phone), "Số điện thoại đã tồn tại");
                        return View("Edit", data);
                    }
                }
                else
                {
                    bool result = CommonDataService.UpdateShipper(data);
                    if (!result)
                    {
                        ModelState.AddModelError(nameof(data.Phone), "Số điện thoại đã tồn tại");
                        return View("Edit", data);
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Lỗi hệ thống");
                return View("Edit",data);
            }
        }
        public IActionResult Delete(int id=0)
        {
            @ViewBag.Title = "Xóa thông tin người giao hàng";
            if(Request.Method == "POST")
            {
                CommonDataService.DeleteShipper(id);
                return RedirectToAction("Index");
            }
            var data = CommonDataService.GetShipper(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }
    }
}
