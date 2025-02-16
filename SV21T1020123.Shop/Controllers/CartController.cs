using Microsoft.AspNetCore.Mvc;
using SV21T1020123.DomainModels;
using SV21T1020123.Shop.Models;

namespace SV21T1020123.Shop.Controllers
{
    public class CartController : Controller
    {
        private const string SHOPPING_CART = "ShoppingCart";
        private const string SHOPPING_COUNT = "ShoppingCount";
        public IActionResult Index()
        {
            var shoppingCart = GetShoppingCart();
            return View(shoppingCart);
        }
        public IActionResult AddToCart(int id = 0, int quantity = 1)
        {
            var shoppingCart = GetShoppingCart();
            var data = SV21T1020123.BusinessLayers.ProductDataService.GetProduct(id);
                var existsProduct = shoppingCart.FirstOrDefault(m => m.ProductID == id);
                if (existsProduct == null)
                {
                    CartItem cartItem = new CartItem()
                    {
                        ProductID = data.ProductID,
                        ProductName = data.ProductName,
                        Photo = data.Photo,
                        Quantity = quantity,
                        Unit = data.Unit,
                        SalePrice = data.Price
                    };
                    shoppingCart.Add(cartItem);
                }
                else
                {
                    existsProduct.Quantity += quantity;
                    if (existsProduct.Quantity == 0)
                    {
                        shoppingCart.RemoveAt(shoppingCart.FindIndex(m => m.ProductID == id));
                    }
                }
                ApplicationContext.SetSessionData(SHOPPING_COUNT, shoppingCart.Count);
                ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
                return RedirectToAction("Index");
        }
            public IActionResult RemoveFromCart(int id = 0)
        {
            var shoppingCart = GetShoppingCart();
            int index = shoppingCart.FindIndex(m => m.ProductID == id);
            if (index >= 0)
                shoppingCart.RemoveAt(index);
            ApplicationContext.SetSessionData(SHOPPING_COUNT, shoppingCart.Count);
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return View("Index", shoppingCart);
        }
        public IActionResult ClearCart()
        {
            var shoppingCart = GetShoppingCart();
            shoppingCart.Clear();
            ApplicationContext.SetSessionData(SHOPPING_COUNT, shoppingCart.Count);
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return View("Index", shoppingCart);
        }
        public IActionResult ComfirmOrder(string deliveryAddress)
        {
            var shoppingCart = GetShoppingCart();
            if(shoppingCart.Count == 0)
            {
                ModelState.AddModelError("Error", "Giỏ hàng trống! ");
            }
            if (string.IsNullOrWhiteSpace(deliveryAddress))
            {
                ModelState.AddModelError("Error2", "Vui lòng nhập địa chỉ giao hàng!");
            }
            if (!ModelState.IsValid)
            {
                return View("Index", shoppingCart);

            }
            if (User.GetUserData() == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var customer = SV21T1020123.BusinessLayers.CommonDataService.GetCustomer(int.Parse(User.GetUserData().UserId));
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
            int id = SV21T1020123.BusinessLayers.OrderDataService.CustomerInitOrder(customer.CustomerID,customer.Province,deliveryAddress,orderDetails);
            ClearCart(); 
            return RedirectToAction("Details", "Order", new {id});
        }

        private List<CartItem> GetShoppingCart()
        {
            var shoppingCart = ApplicationContext.GetSessionData<List<CartItem>>(SHOPPING_CART);
            if (shoppingCart == null)
            {
                shoppingCart = new List<CartItem>();
                ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            }
            return shoppingCart;
        }
    }
}
