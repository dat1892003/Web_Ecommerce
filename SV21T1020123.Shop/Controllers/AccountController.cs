using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SV21T1020123.BusinessLayers;
using SV21T1020123.DomainModels;

namespace SV21T1020123.Shop.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            @ViewBag.UserName = username;
            ViewBag.Username = username;
            //Kiểm tra thông tin đầu vào
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(username))
            {
                ModelState.AddModelError("Error", "Nhập đầy đủ tên và mật khẩu");
                return View();
            }
            //TODO: Kiểm tra xem username và passwor có hợp lệ không ?
            var data = BusinessLayers.UserAccountService.Authorize(BusinessLayers.UserAccountService.UserTypes.Customer, username, password);
            if (data == null)
            {
                ModelState.AddModelError("Error", "Thông tin đăng nhập không đúng");
                return View();
            }
            // Đăng nhập thành công
            var userData = new WebUserData()
            {
                UserId = data.UserID,
                UserName = data.UserName,
                Photo = data.Photo,
                DisplayName = data.DisplayName,
                Roles = data.Role.Split(',').ToList()
            };
            //ghi nhận đăng nhập
            await HttpContext.SignInAsync(userData.CreatePrincipal());
            //quay về trang chủ
            return RedirectToAction("Index", "Home");
        }
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrEmpty(oldPassword)|| string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                ModelState.AddModelError("Error", "Vui lòng nhập đầy đủ thông tin!");
                return View();
            }
            var user = SV21T1020123.BusinessLayers.UserAccountService.Authorize(BusinessLayers.UserAccountService.UserTypes.Customer,
                User.GetUserData().UserName, oldPassword);
            if (user == null)
            {
                ModelState.AddModelError("Error", "Mật khẩu cũ không chính xác !");
                return View(); ;
            }
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("Error", "Mật khẩu mới không trùng khớp !");
                return View();
            }
            bool result = SV21T1020123.BusinessLayers.UserAccountService.ChangePassword(BusinessLayers.UserAccountService.UserTypes.Customer
                , User.GetUserData().UserName, newPassword);
            if (!result)
            {
                ModelState.AddModelError("Error", "Lỗi cơ sở dữ liệu !");
            }
            else
            {
                ModelState.AddModelError("Error", "Đổi mật khẩu thành công !");
            }
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        public IActionResult Edit()
        {
            var data = SV21T1020123.BusinessLayers.CommonDataService.GetCustomer(int.Parse(User.GetUserData().UserId));
            return View(data);
        }
        [HttpPost]
        public IActionResult Edit(Customer data)
        {
            if (string.IsNullOrWhiteSpace(data.CustomerName))
                ModelState.AddModelError(nameof(data.CustomerName), "Tên khách hàng không được để trống");
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
                    //Cập nhập
                    bool result = CommonDataService.UpdateCustomer(data);
                    if (!result)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Email đã tồn tại");
                        return View("Edit", data);
                    }
                ModelState.AddModelError("Error", "Thay đổi thông tin thành công");
                return View("Edit", data);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Lỗi hệ thống");
                return View("Edit", data);
            }
        }
        public IActionResult AccessDenined()
        {
            return View();
        }
        public IActionResult SignUp()
        {
            Customer data = new Customer() { IsLocked = false};
            return View(data);
        }
        public IActionResult Save(Customer data)
        {
            if (string.IsNullOrWhiteSpace(data.CustomerName))
                ModelState.AddModelError(nameof(data.CustomerName), "Tên khách hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.ContactName))
                ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch không được để trống");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Số điện thoại không được để trống");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Email không được để trống");
            if (string.IsNullOrWhiteSpace(data.Address))
                ModelState.AddModelError(nameof(data.Address), "Vui lòng nhập địa chỉ của khách hàng");
            if (string.IsNullOrWhiteSpace(data.Province))
                ModelState.AddModelError(nameof(data.Province), "Hãy chọn tĩnh/thành cho khách hàng");
            if (string.IsNullOrWhiteSpace(data.Password))
                ModelState.AddModelError(nameof(data.Password), "Mật khẩu không được để trống");
            if (!ModelState.IsValid)
            {
                return View("SignUp", data);
            }
            else
            {
                try
                {
                    int id = UserAccountService.Singup(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError("Error", "Tài khoản đã tồn tại trong hệ thống");
                        return View("SignUp", data);
                    }
                    else
                    {
                        ModelState.AddModelError("Error", "Đăng ký thành công");
                        return View("SignUp", data);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Error", "Lỗi cơ sở dữ liệu");
                    return View("SignUp", data);
                }
            }
        }
    }
}
