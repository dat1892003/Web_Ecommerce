using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020123.BusinessLayers;
using SV21T1020123.DomainModels;
using SV21T1020123.Web.AppCodes;
using SV21T1020123.Web.Models;

namespace SV21T1020123.Web.Controllers
{
    [Authorize(Roles =$"{WebUserRoles.ADMINSTATOR}")]
    public class EmployeeController : Controller
    {
        private const int PAGE_SIZE = 9;
        private const string EMPLOYEE_SEARCH_CONDITION = "EmployeeSearchCondition";
        public IActionResult Index()
        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(EMPLOYEE_SEARCH_CONDITION);
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
            var data = CommonDataService.ListOfEmployees(out rowCount, condition.Page, condition.PageSize, condition.SearchValue);
            EmployeeSearchResult model = new EmployeeSearchResult()
            {
                Data = data,
                SearchValue = condition.SearchValue ?? "",
                Page = condition.Page,
                PageSize = condition.PageSize,
                RowCount = rowCount
            };
            ApplicationContext.SetSessionData(EMPLOYEE_SEARCH_CONDITION,condition);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhân viên mới";
            var data = new Employee()
            {
                EmployeeID = 0,
                IsWorking = true,
            };
            return View("Edit", data);
        }
        public IActionResult Edit(int id=0)
        {
            ViewBag.Title = "Cập nhật thông tin nhân viên";
            var data = CommonDataService.GetEmployee(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }
        [HttpPost]
        public IActionResult Save(Employee data, string _BirthDate, IFormFile _Photo)
        {
            DateTime defaultDate = new DateTime(1, 1, 1);
            ViewBag.Title = data.EmployeeID == 0 ? "Bổ sung nhân viên mới" : "Cập nhật thông tin nhân viên";
            if (string.IsNullOrWhiteSpace(data.FullName))
                ModelState.AddModelError(nameof(data.FullName), "Tên nhân viên không được để trống");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Email không được để trống");
            if (string.IsNullOrWhiteSpace(data.Address))
                ModelState.AddModelError(nameof(data.Address), "Địa chỉ không được để trống");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập số điện thoại");
            if (string.IsNullOrWhiteSpace(data.Photo) && _Photo == null)
                ModelState.AddModelError(nameof(data.Photo), "Nhân viên phải có ảnh");
            if (_BirthDate.ToDateTime() == defaultDate)
                ModelState.AddModelError(nameof(data.BirthDate), "Ngày không hợp lệ");

            if (!ModelState.IsValid)
            {
                return View("Edit",data);
            }
            //xử lí ngày sinh
            DateTime? d = _BirthDate.ToDateTime();
            if (d != null)
            {
                DateTime minDate = new DateTime(1753, 1, 1);
                DateTime maxDate = new DateTime(9999, 12, 31);
   
                data.BirthDate = d.Value; 
                if (data.BirthDate < minDate || data.BirthDate > maxDate)
                {
                    ModelState.AddModelError(nameof(data.BirthDate), "Ngày sinh không hợp lệ");
                    return View("Edit", data);
                }
            }
            if(_Photo != null)
            {
                string fileName = $"{DateTime.Now.Ticks}-{_Photo.FileName}";
                string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images\employees", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    _Photo.CopyTo(stream);
                }
                data.Photo = fileName;
            }
            try
            {
                if (data.EmployeeID == 0)
                {
                    int id = CommonDataService.AddEmployee(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Email đã tồn tại");
                        return View("Edit", data);
                    }
                }
                else
                {
                    bool result = CommonDataService.UpdateEmployee(data);
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
            ViewBag.Title = "Xóa thông tin nhân viên";
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteEmployee(id);
                return RedirectToAction("Index");
            }
            var data = CommonDataService.GetEmployee(id);
            if (data == null) {
                return RedirectToAction("Index");
            }     
            return View(data);
        }
    }
}
