using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020123.BusinessLayers;
using SV21T1020123.DomainModels;
using SV21T1020123.Web.AppCodes;
using SV21T1020123.Web.Models;

namespace SV21T1020123.Web.Controllers
{
    [Authorize]
    public class SupplierController : Controller
    {
        public const int PAGE_SIZE = 10;
        private const string SUPPLIER_SEARCH_CONDITION = "SupplierSearchCondition";
        public IActionResult Index()
        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(SUPPLIER_SEARCH_CONDITION);
            if (condition == null)
                condition = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            return View(condition);
        }
        public IActionResult Search(PaginationSearchInput condition)
        {
            int rowCount;
            var data = CommonDataService.ListOfSuppliers(out rowCount,condition.Page,condition.PageSize,condition.SearchValue);
            SupplierSearchResult model = new SupplierSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,  
                RowCount = rowCount,
                SearchValue=condition.SearchValue?? "",
                Data = data
            };
            ApplicationContext.SetSessionData(SUPPLIER_SEARCH_CONDITION, model);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhà cung cấp mới";
            var data = new Supplier()
            {
                SupplierID = 0,
            }; 
            return View("Edit", data);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin nhà cung cấp";
            var data = CommonDataService.GetSupplier(id);
            if(data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }
        [HttpPost]
        public IActionResult Save(Supplier data) {
            //TODO: Kiểm soát dữ liệu đầu vào
            ViewBag.Title = data.SupplierID == 0 ? "Bổ sung nhà cung cấp mới" : "Cập nhật thông nhà cung cấp";
            //Kiểm tra dữ liệu nếu không hợp lệ tạo ra một thông báo lỗi lưu thông báo lỗi vào ModelState
            if (string.IsNullOrWhiteSpace(data.SupplierName))
                ModelState.AddModelError(nameof(data.SupplierName), "Tên nhà cung cấp không được để trống");
            if (string.IsNullOrWhiteSpace(data.ContactName))
                ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch không được để trống");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Số điện thoại không được để trống");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Email không được để trống");
            if (string.IsNullOrWhiteSpace(data.Address))
                ModelState.AddModelError(nameof(data.Address), "Vui lòng nhập địa chỉ của nhà cung cấp");
            if (string.IsNullOrWhiteSpace(data.Province))
                ModelState.AddModelError(nameof(data.Province), "Hãy chọn tĩnh/thành cho nhà cung cấp");
            //Dựa vào IsVaild của Model để biết có tồn tại lỗi hay không 
            if (!ModelState.IsValid)
            {
                return View("Edit", data);
            }
            try
            {
                if (data.SupplierID == 0)
                {
                    int id = CommonDataService.AddSupplier(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Email đã tồn tại");
                        return View("Edit", data);
                    }
                }
                else
                {
                    bool result = CommonDataService.UpdateSupplier(data);
                    if (!result) 
                    {
                        ModelState.AddModelError(nameof(data.Email), "Email đã tồn tại");
                        return View("Edit", data);
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Lỗi hệ thống");
                return View("Edit", data);
            }
        }
        public IActionResult Delete(int id = 0)
        {
            ViewBag.Title = "Xóa thông tin nhà cung cấp";
            if(Request.Method == "POST")
            {
                CommonDataService.DeleteSupplier(id);
                return RedirectToAction("Index");
            }
            var data = CommonDataService.GetSupplier(id); 
            if (data == null) 
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }

    }
}
