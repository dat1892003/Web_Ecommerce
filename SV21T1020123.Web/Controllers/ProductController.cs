using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020123.BusinessLayers;
using SV21T1020123.DomainModels;
using SV21T1020123.Web.AppCodes;
using SV21T1020123.Web.Models;

namespace SV21T1020123.Web.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string PRODUCT_SEARCH_CONDITION = "ProductSearchCondition";
        public IActionResult Index()
        {
            ProductSearchInput? condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
            if (condition == null)
            {
                condition = new ProductSearchInput()
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
        public IActionResult Search(ProductSearchInput condition)
        {
            int rowCount;
            var data = ProductDataService.ListProducts(out rowCount, condition.Page, condition.PageSize, condition.SearchValue, condition.CategoryID, 
                                                        condition.SupplierID, condition.MinPrice, condition.MaxPrice);
            ProductSearchResult model = new ProductSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                CategoryID = condition.CategoryID,
                SupplierID = condition.SupplierID,
                MaxPrice = condition.MaxPrice,
                MinPrice =condition.MinPrice,
                Data = data,
                RowCount = rowCount,
            };
            ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);
            return View(model);
        }
        [HttpPost]
        public IActionResult Save(Product data, string _SupplierID, string _CategoryID, string _Price, IFormFile _Photo)
        {
            ViewBag.Title = data.ProductID == 0 ? "Bổ sung mặt hàng mới cấp mới" : "Cập nhật thông tin mặt hàng";
            if (string.IsNullOrWhiteSpace(data.ProductName))
                ModelState.AddModelError(nameof(data.ProductName), "Tên mặt hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.ProductDescription))
                ModelState.AddModelError(nameof(data.ProductDescription), "Miêu tả cho mặt hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.Unit))
                ModelState.AddModelError(nameof(data.Unit), "Đơn vị tính không được để trống");
            if (!int.TryParse(_SupplierID, out _) || int.Parse(_SupplierID) == 0) 
                ModelState.AddModelError(nameof(data.SupplierID), "Nhà cung cấp không được để trống");
            if (!int.TryParse(_CategoryID, out _) || int.Parse(_CategoryID) == 0)
                ModelState.AddModelError(nameof(data.CategoryID), "Loại hàng không được để trống");
            if (!decimal.TryParse(_Price,out _) || decimal.Parse(_Price) < 0)
                ModelState.AddModelError(nameof(data.Price), "Giá không hợp lệ");
            if (string.IsNullOrWhiteSpace(data.Photo) && _Photo == null)
                ModelState.AddModelError(nameof(data.Photo), "Mặt hàng phải có ảnh");
            data.Price = decimal.Parse(_Price);
            data.SupplierID = int.Parse(_SupplierID);
            data.CategoryID = int.Parse(_CategoryID);
            if (!ModelState.IsValid)
            {
                return View("Edit",data);
            }           
            if (_Photo != null)
            {
                string fileName = $"{DateTime.Now.Ticks}-{_Photo.FileName}";
                string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images\products", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    _Photo.CopyTo(stream);
                }
                data.Photo = fileName;
            }
            if (data.ProductID == 0)
            {
                int id = ProductDataService.AddProduct(data);
                if (id <=0 )
                {
                    ModelState.AddModelError(nameof(data.ProductName), "Tên hàng hóa đã tồn tại");
                    return View("Edit",data);
                }
                else
                {
                    var product = ProductDataService.GetProduct(id);
                    return View("Edit", product);
                }
            }
            else
            {
                bool result = ProductDataService.UpdateProduct(data);
                if (!result)
                {
                    ModelState.AddModelError(nameof(data.ProductName), "Tên hàng hóa đã tồn tại");
                    return View("Edit", data);
                }   
            }
            return RedirectToAction("Index");
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung mặt hàng mới";
            var data = new Product()
            {
                ProductID = 0,
                IsSelling = true,
            };
            return View("Edit", data);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin mặt hàng";
            var data = ProductDataService.GetProduct(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }
        public IActionResult Delete(int id = 0) {
            ViewBag.Title = "Xóa thông tin mặt hàng";
            if(Request.Method == "POST")
            {
                ProductDataService.DeleteProduct(id);
                return RedirectToAction("Index");
            }
            var data = ProductDataService.GetProduct(id);
            if(data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }
        public IActionResult Photo(int id = 0, string method = "", int photoId = 0)
        {
            var data = new ProductPhoto();
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung ảnh cho mặt hàng";
                    data.PhotoID = photoId;
                    data.ProductID = id;
                    return View(data);
                case "edit":
                    ViewBag.Tile = "Thay đổi ảnh của mặt hàng";
                    data = ProductDataService.GetPhoto(id);
                    return View(data);
                case "delete":
                    //TODO: Xóa ảnh (xóa trực tiếp không cần confirm)
                    ProductDataService.DeletePhoto(photoId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult SavePhoto(ProductPhoto data, IFormFile _Photo)
        {
            ViewBag.Title = data.PhotoID == 0 ? "Bổ sung ảnh mới cho mặt hàng" : "Cập nhật thông tin ảnh của mặt hàng";
            if (string.IsNullOrEmpty(data.Photo) && _Photo == null)
               ModelState.AddModelError(nameof(data.Photo), "Ảnh không được để trống");
            if (string.IsNullOrEmpty(data.Description))
               ModelState.AddModelError(nameof(data.Description), "Mô tả không được để trống");
            if (data.DisplayOrder <= 0)
               ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự phải hiển thị phải là số dương lớn hơn 0");
            
            if (!ModelState.IsValid)
            {
                return View("Photo", data);
            }

            try { 
            if (_Photo != null)
            {
                string fileName = $"{DateTime.Now.Ticks}-{_Photo.FileName}";
                string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images\products", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    _Photo.CopyTo(stream);
                }
                data.Photo = fileName;
            }
            if (data.PhotoID == 0)
            {
                long id = ProductDataService.AddPhoto(data);
                if (id <= 0)
                {
                    ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị đã tồn tại");
                    return View("Photo", data);
                }    
            }
            else
            {
                bool result = ProductDataService.UpdatePhoto(data);
                if(!result)
                {
                    ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị đã tồn tại");
                    return View("Photo", data);
                }
            }    
            var productData = ProductDataService.GetProduct(data.ProductID);
            return View("Edit", productData);
            }
            catch (Exception ex) {
                ModelState.AddModelError("Error", "Lỗi hệ thống");
                return View("Photo", data);
            }
        }
        public IActionResult Attribute(int id = 0, string method = "", int attributeId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính cho mặt hàng";
                    var data = new ProductAttribute()
                    {
                        AttributeID = attributeId,
                        ProductID = id,
                    };
                    return View(data);
                case "edit":
                    ViewBag.Tile = "Thay đổi thuộc tính của mặt hàng";
                    var dataEdit = ProductDataService.GetAttribute(attributeId);
                    if(dataEdit == null)
                    {
                        return RedirectToAction("Index");
                    }
                    return View(dataEdit);
                case "delete":
                    //TODO: Xóa ảnh (xóa trực tiếp không cần confirm)
                    ProductDataService.DeleteAttribute(attributeId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult SaveAttribute(ProductAttribute data)
        {
            ViewBag.Title = data.AttributeID == 0 ? "Bổ sung mặt hàng mới cấp mới" : "Cập nhật thông tin mặt hàng";

            if (string.IsNullOrEmpty(data.AttributeName))
               ModelState.AddModelError(nameof(data.AttributeName), "Tên thuộc tính không được để trống");
            if (string.IsNullOrEmpty(data.AttributeValue))
               ModelState.AddModelError(nameof(data.AttributeValue), "Tên thuộc tính không được để trống");
            if (data.DisplayOrder <= 0)
               ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự phải là số dương lớn hơn 0");


            if(ModelState.IsValid == false)
            {
                return View("Attribute",data);
            }

            try { 
            if (data.AttributeID == 0) 
            {
                long id = ProductDataService.AddAttribute(data);
                if (id == -1)
                {
                    ModelState.AddModelError(nameof(data.AttributeName),"Tên thuộc tính đã tồn tại");
                    return View("Attribute", data);
                }
                else if (id == -2)
                {
                    ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị bị trùng lặp");
                    return View("Attribute", data);
                }
            }
            else
            {
                bool result = ProductDataService.UpdateAttribute(data);
                if (!result) {
                    ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự bị trùng lặp");
                    return View("Attribute", data);
                }
            }
            var dataProduct = ProductDataService.GetProduct(data.ProductID);
            return View("Edit",dataProduct);
            }
            catch (Exception ex) {
                ModelState.AddModelError("Error", "Lỗi hệ thống");
                return View("Attribute", data);
            }
        }
    }
}
