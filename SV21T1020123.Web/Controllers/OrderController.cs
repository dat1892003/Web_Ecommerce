using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020123.BusinessLayers;
using SV21T1020123.DomainModels;
using SV21T1020123.Web.AppCodes;
using SV21T1020123.Web.Models;
using System.Globalization;
namespace SV21T1020123.Web.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private const int PAGE_SIZE = 25;
        private const string SHOPPING_CART = "ShoppingCart";
        private const string ORDER_SEARCH_CONDITION = "OrderSearchCondition";
        //Số mặt hàng hiển thị trong 1 trang
        private const int PRODUCT_PAGE_SIZE = 5;
        //Tên biến session lưu trữ điều kiện tìm kiếm mặt hàng khi lập đơn hàng
        private const string PRODUCT_SEARCH_CONDITION = "ProductSearchSale";
        public IActionResult Index()
        {
            OrderSearchInput? condition = ApplicationContext.GetSessionData<OrderSearchInput>(ORDER_SEARCH_CONDITION);

            if (condition == null)
            {
                condition = new OrderSearchInput()
                {
                    SearchValue = "",
                    Page = 1,
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
            var data = OrderDataService.ListOrders(out rowCount, conditon.Page, conditon.PageSize, conditon.Status, conditon.FromTime, conditon.ToTime, conditon.SearchValue);
            OrderSearchResult model = new OrderSearchResult() 
            { 
                data = data,
                TimeRange=conditon.TimeRange,
                RowCount = rowCount,
                Page = conditon.Page,
                PageSize = conditon.PageSize,
                SearchValue = conditon.SearchValue,
                Status = conditon.Status,
            };
            ApplicationContext.SetSessionData(ORDER_SEARCH_CONDITION, conditon);
            return View(model);
        }
        public IActionResult Create()
        {
            var condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
            if (condition == null)
            {
                condition = new ProductSearchInput(){
                    Page = 1,
                    PageSize = PRODUCT_PAGE_SIZE,
                    SearchValue ="",
                };
            }
            return View(condition);
        }
        public IActionResult SearchProduct(ProductSearchInput condition)
        {
            int rowCount = 0;
            var data = ProductDataService.ListProducts(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            var model = new ProductSearchResult() { 
                Data = data,
                Page = condition.Page,
                PageSize = condition.PageSize,  
                RowCount=rowCount,
                SearchValue = condition.SearchValue ?? ""
            };
            ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION,condition);
            return View(model);
        }
        public IActionResult Accept(int id)
        {
            bool result = OrderDataService.AcceptOrder(id,int.Parse(User.GetUserData().UserId));
            var order = OrderDataService.GetOrder(id);
            if (order == null)
                return RedirectToAction("Index");
            var details = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = details
            };
            return View("Details",model);
        }
        public IActionResult Finish(int id)
        {
            bool result = OrderDataService.FinishOrder(id);
            var order = OrderDataService.GetOrder(id);
            if (order == null)
                return RedirectToAction("Index");
            var details = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = details
            };
            return View("Details", model);
        }
        public IActionResult Cancel(int id) 
        {
            bool result = OrderDataService.CancelOrder(id, int.Parse(User.GetUserData().UserId));
            var order = OrderDataService.GetOrder(id);
            if (order.EmployeeID == null)
            {
                order.EmployeeID = int.Parse(User.GetUserData().UserId);
            }
            if (order == null)
                return RedirectToAction("Index");
            var details = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = details
            };
            if (!result)
            {
                ModelState.AddModelError("Error", "Không thể hủy bỏ đơn hàng này"); 
            }
            return View("Details", model);
        }
        public IActionResult Delete(int id) 
        {
            bool result = OrderDataService.DeleteOrder(id);
            if (!result)
            {
                var order = OrderDataService.GetOrder(id);
                if (order == null)
                    return RedirectToAction("Index");
                var details = OrderDataService.ListOrderDetails(id);
                var model = new OrderDetailModel()
                {
                    Order = order,
                    Details = details
                };
                ModelState.AddModelError("Error", "Không thể xóa đơn hàng này");
                return View("Details", model);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public IActionResult Reject(int id)
        {
            bool result = OrderDataService.RejectOrder(id, int.Parse(User.GetUserData().UserId));
            var order = OrderDataService.GetOrder(id);
            if (order.EmployeeID == null)
            {
                order.EmployeeID = int.Parse(User.GetUserData().UserId);
            }
            if (order == null)
                return RedirectToAction("Index");
            var details = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = details
            };
            if (!result) 
            {
                ModelState.AddModelError("Error", "Không thể từ chối đơn hàng này");
            }
            return View("Details", model);
        }
        public IActionResult Init(int CustomerID = 0, string deliveryProvince = "", string deliveryAddress = "")
        {
            var shoppingCart =GetShoppingCart();
            if (shoppingCart.Count == 0)
                return Json("Giỏ hàng trống. Vui lòng nhập mặt hàng cần bán");
            if (CustomerID == 0 || string.IsNullOrWhiteSpace(deliveryProvince) || string.IsNullOrWhiteSpace(deliveryAddress))
                return Json("Vui lòng nhập đầy đủ tên khách hàng và nơi giao hàng");
            //TODO: Thay bằng ID nhân viên đăng nhập vào hệ thống
            int employeeID = int.Parse(User.GetUserData().UserId); 
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            foreach (var item in shoppingCart)
            {
                orderDetails.Add(new OrderDetail
                {
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    SalePrice = item.SalePrice,
                });
            }
            int orderID = OrderDataService.InitOrder(employeeID,CustomerID,deliveryProvince,deliveryAddress,orderDetails);
            ClearCart();
            return Json(orderID);
        }
        public IActionResult Details(int id = 0 )
        {
            var order = OrderDataService.GetOrder(id);
            if (order == null)
                return RedirectToAction("Index");
            var details = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel() {
                Order = order,
                Details = details
            };
            return View(model);
        }
        public IActionResult EditDetail(int id = 0, int productId = 0)
        {
            var data = OrderDataService.GetOrderDetail(id, productId);
            return View(data);
        }
        public IActionResult UpdateDetail(int id, OrderDetail data) {
            if (data.SalePrice >= 0)
            {
                bool result = OrderDataService.SaveOrderDetail(id, data.ProductID, data.Quantity, data.SalePrice);
            }
            var order = OrderDataService.GetOrder(id);
            var details = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = details
            };
            return View("Details", model);
        }
        public IActionResult DeleteDetail(int id, int productId)
        {
            bool result = OrderDataService.DeleteOrderDetail(id, productId);
            var order = OrderDataService.GetOrder(id);
            var details = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = details
            };
            if (!result)
            {
                ModelState.AddModelError("ErrorEdit", "Không thể xóa hàng hóa ra khỏi đơn hàng này");
            }
            return View("Details", model);
        }
        public IActionResult Shipping(int id = 0)
        {
            var order = OrderDataService.GetOrder(id);
            if (order == null)
                return RedirectToAction("Index");
            var details = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = details
            };
            return View(model);

        }
        [HttpPost]
        public IActionResult ShippingAccept(int id =0, int shipperID = 0)
        {
            var order = OrderDataService.GetOrder(id);
            if (order == null)
                return RedirectToAction("Index");
            var details = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = details
            };
            if (shipperID == 0)
            {
                return View("Details", model);
            }
            bool result = OrderDataService.ShipOrder(id, shipperID);
            if (!result)
            {
                ModelState.TryAddModelError("Error", "Không thể chuyển giao hàng cho đơn hàng này");
                return View("Details", model);
            }
            else
            {
                order = OrderDataService.GetOrder(id);
                model = new OrderDetailModel()
                {
                    Order = order,
                    Details = details
                };
                return View("Details", model);
            }
        }
        public IActionResult AddToCart(CartItem item)
        {
            if (item.SalePrice < 0 || item.Quantity <= 0)
                return Json("Giá bán không hợp lệ");
            var shoppingCart = GetShoppingCart();
            var existsProduct = shoppingCart.FirstOrDefault(m => m.ProductID == item.ProductID);
            if (existsProduct == null)
            {
                shoppingCart.Add(item);
            }
            else
            {
                existsProduct.Quantity += item.Quantity;
                existsProduct.SalePrice = item.SalePrice;
            }
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }
        public IActionResult ClearCart()
        {
            var shoppingCart = GetShoppingCart();
            shoppingCart.Clear();
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }
        public IActionResult RemoveFromCart(int id = 0)
        {
            var shoppingCart = GetShoppingCart();
            int index = shoppingCart.FindIndex(m => m.ProductID == id);
            if (index >= 0)
                shoppingCart.RemoveAt(index);
            ApplicationContext.SetSessionData(SHOPPING_CART,shoppingCart);
            return Json("");
        }
        private List<CartItem> GetShoppingCart()
        {
            var shoppingCart = ApplicationContext.GetSessionData<List<CartItem>>(SHOPPING_CART);
            if(shoppingCart == null)
            {
                shoppingCart= new List<CartItem>();
                ApplicationContext.SetSessionData(SHOPPING_CART,shoppingCart);
            }
            return shoppingCart;
        }
        public IActionResult ShoppingCart()
        {
            return View(GetShoppingCart());
        }
    }
}
