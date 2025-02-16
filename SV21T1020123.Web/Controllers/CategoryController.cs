using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020123.BusinessLayers;
using SV21T1020123.DomainModels;
using SV21T1020123.Web.AppCodes;
using SV21T1020123.Web.Models;

namespace SV21T1020123.Web.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        public const int PAGE_SIZE = 10;
        private const string CATEGORY_SEARCH_CONDITION = "CategorySearchCondition";
        public IActionResult Index()
        {
            PaginationSearchInput? conditon = ApplicationContext.GetSessionData<PaginationSearchInput>(CATEGORY_SEARCH_CONDITION);
            if (conditon == null)
                conditon = new PaginationSearchInput() 
                { 
                    PageSize = PAGE_SIZE,  
                    Page = 1,
                    SearchValue = ""
                };
            return View(conditon);
        }
        public IActionResult Search(PaginationSearchInput conditon) 
        {
            int rowCount;
            var data = CommonDataService.ListOfCategories(out rowCount, conditon.Page, conditon.PageSize, conditon.SearchValue);
            CategorySearchResult model = new CategorySearchResult() 
            { 
                Data = data,
                SearchValue=conditon.SearchValue ?? "",
                PageSize = conditon.PageSize,
                Page = conditon.Page,
                RowCount = rowCount
            };
            ApplicationContext.SetSessionData(CATEGORY_SEARCH_CONDITION, model);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung loại hàng mới";
            var data = new Category
            {
                CategoryID = 0,
            };
            return View("Edit", data);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin loại hàng";
            var data = CommonDataService.GetCategory(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }
        [HttpPost]
        public IActionResult Save(Category data)
        {   
            //TODO: Kiểm soát dữ liệu đầu vào
            ViewBag.Title = data.CategoryID == 0 ? "Bổ sung loại hàng mới" : "Cập nhật thông loại hàng";
            //Kiểm tra dữ liệu nếu không hợp lệ tạo ra một thông báo lỗi lưu thông báo lỗi vào ModelState
            if (string.IsNullOrWhiteSpace(data.CategoryName))
                ModelState.AddModelError(nameof(data.CategoryName), "Tên loại hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.Description))
                ModelState.AddModelError(nameof(data.Description), "Nhập miêu tả cho loại hàng");
            if (!ModelState.IsValid)
            {
                return View("Edit", data);
            }
            try 
            {
                if (data.CategoryID == 0)
                {
                    int id = CommonDataService.AddCategory(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.CategoryName), "Tên loại hàng đã tồn tại");
                    return View("Edit", data);
                    }

                }
                else
                {
                    bool result = CommonDataService.UpdateCategory(data);
                    if (!result)
                        {
                            ModelState.AddModelError(nameof(data.CategoryName), "Tên loại hàng đã tồn tại");
                            return View("Edit", data);
                        }
                }
                return RedirectToAction("Index");
            } catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Lỗi hệ thống");
                return View("Edit", data);
            }
        }
        public IActionResult Delete(int id=0)
        {
            ViewBag.Title = "Xóa thông tin loại hàng";
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteCategory(id);
                return RedirectToAction("Index");
            }
            var data = CommonDataService.GetCategory(id);
            if (data == null) 
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }
    }
}
