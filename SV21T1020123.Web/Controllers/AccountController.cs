using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace SV21T1020123.Web.Controllers
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
            ViewBag.Username = username;
            //Kiểm tra thông tin đầu vào
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(username))
            {
                ModelState.AddModelError("Error", "Nhập đầy đủ tên và mật khẩu");
                return View();
            }
            //TODO: Kiểm tra xem username và passwor có hợp lệ không ?
            var data = BusinessLayers.UserAccountService.Authorize(BusinessLayers.UserAccountService.UserTypes.Employee, username, password);
            if ( data == null)
            {
                ModelState.AddModelError("Error", "Thông tin đăng nhập không đúng");
                return View();
            }
            // Đăng nhập thành công
            var userData = new WebUserData() {
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
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        public IActionResult AccessDenined(string message) 
        {
            return View();
        }
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword)
                || string.IsNullOrEmpty(confirmPassword))
            {
                ModelState.AddModelError("Error", "Vui lòng nhập đầy đủ thông tin!");
                return View();
            }
            var user = SV21T1020123.BusinessLayers.UserAccountService.Authorize(BusinessLayers.UserAccountService.UserTypes.Employee ,
                User.GetUserData().UserName, oldPassword);
            if ( user == null) 
            {
                ModelState.AddModelError("Error", "Mật khẩu cũ không chính xác !");
                return View(); ;
            }
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("Error", "Mật khẩu mới không trùng khớp !");
                return View();
            }
            bool result = SV21T1020123.BusinessLayers.UserAccountService.ChangePassword(BusinessLayers.UserAccountService.UserTypes.Employee
                , User.GetUserData().UserName, newPassword);
            if (!result) 
            {
                ModelState.AddModelError("Error", "Lỗi cơ sở dữ liệu !");
            } else
            {
                ModelState.AddModelError("Error", "Đổi mật khẩu thành công !");
            }
            return View();
        }
    }
}
