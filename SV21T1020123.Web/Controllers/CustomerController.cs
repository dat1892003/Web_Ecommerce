using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020123.BusinessLayers;
using SV21T1020123.DomainModels;
using SV21T1020123.Web.AppCodes;
using SV21T1020123.Web.Models;

namespace SV21T1020123.Web.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private const int PAGE_SIZE= 20;
        private const string CUSTOMER_SEARCH_CONDITION = "CustomerSearchCondition";
        public IActionResult Index()
        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(CUSTOMER_SEARCH_CONDITION);
            if (condition == null)
                condition = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            return View(condition);
        }
        public IActionResult Search(PaginationSearchInput conditon)
        {
            int rowCount;
            var data = CommonDataService.ListOfCustomers(out rowCount,conditon.Page,conditon.PageSize,conditon.SearchValue);
            CustomerSearchResult model = new CustomerSearchResult()
            {
                Page = conditon.Page,
                PageSize = conditon.PageSize,
                SearchValue = conditon.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(CUSTOMER_SEARCH_CONDITION, conditon);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung khách hàng mới";
            var data = new Customer()
            {
                CustomerID = 0,
                IsLocked = false,
            };
            return View("Edit", data);
        }
        public IActionResult Edit(int id=0)
        {
            ViewBag.Title = "Cập nhật thông tin khách hàng";
            var data = CommonDataService.GetCustomer(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }
        [HttpPost]
        public IActionResult Save(Customer data) // (int CustomerID, string CustomerName , string ContactName,...) <Cách viết liệt kê>
        {
            //TODO: Kiểm soát dữ liệu đầu vào
            ViewBag.Title = data.CustomerID == 0 ? "Bổ sung khách hàng mới" :"Cập nhật thông tin khách hàng";
            //Kiểm tra dữ liệu nếu không hợp lệ tạo ra một thông báo lỗi lưu thông báo lỗi vào ModelState
            if (string.IsNullOrWhiteSpace(data.CustomerName))
                ModelState.AddModelError(nameof(data.CustomerName),"Tên khách hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.ContactName))
                ModelState.AddModelError(nameof(data.ContactName), "Tên khách hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Số điện thoại không được để trống");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Email không được để trống");
            if (string.IsNullOrWhiteSpace(data.Address))
                ModelState.AddModelError(nameof(data.Address), "Vui lòng nhập địa chỉ của khách hàng");
            if (string.IsNullOrWhiteSpace(data.Province))
                ModelState.AddModelError(nameof(data.Province), "Hãy chọn tĩnh/thành cho khách hàng");
            //Dựa vào IsVaild của Model để biết có tồn tại lỗi hay không 
            if (!ModelState.IsValid)
            {
                return View("Edit", data);
            }
            try
            {
                if (data.CustomerID == 0)
                {
                    //Thêm 
                    int id = CommonDataService.AddCustomer(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Email đã tồn tại");
                        return View("Edit", data);
                    }
                }
                else
                {
                    //Cập nhập
                    bool result = CommonDataService.UpdateCustomer(data);
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
        public IActionResult Delete(int id=0)
        {
            ViewBag.Title = "Xóa thông tin khách hàng";
            if(Request.Method == "POST")
            {
                CommonDataService.DeleteCustomer(id);
                return RedirectToAction("Index");
            }
            var data = CommonDataService.GetCustomer(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }

    }

}
