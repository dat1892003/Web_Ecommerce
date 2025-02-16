using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace SV21T1020123.Web
{
    /// <summary>
    /// Lưu trữ thông tin người dùng được ghi trong user
    /// </summary>
    public class WebUserData
    {
        public string UserId { get; set; } = "";
        public string UserName { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Photo { get; set; } = "";
        public List<string>? Roles { get; set; }
        private List<Claim> Claims
        {
            get
            {
                List<Claim> claims = new List<Claim>()
                {
                    new Claim(nameof(UserId), UserId),
                    new Claim(nameof(UserName), UserName),
                    new Claim(nameof(DisplayName), DisplayName),
                    new Claim(nameof(Photo), Photo),
                };
                if (Roles != null)
                {
                    foreach (var role in Roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }
                }
                return claims;
            }
        }
        /// <summary>
        /// tạo ra claim principal dựa trên thông tin người dùng (cần lưu trong cookie)
        /// </summary>
        /// <returns></returns>
        public ClaimsPrincipal CreatePrincipal()
        {
            var identity = new ClaimsIdentity(Claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principals = new ClaimsPrincipal(identity);
            return principals;
        }
    }
}
