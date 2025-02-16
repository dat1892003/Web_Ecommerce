using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.FileProviders;

namespace SV21T1020123.Shop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddControllersWithViews()
            .AddMvcOptions(option =>
            {
                option.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            });
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(option =>
                {
                    option.Cookie.Name = "AuthenticationCustomerCookie";        //tên Cookie
                    option.LoginPath = "/Account/Login";                //URL trang đăng nhập
                    option.AccessDeniedPath = "/Account/AccessDenined"; //URL đến trang cấm truy cập 
                    option.ExpireTimeSpan = TimeSpan.FromDays(360);     //Thời gian hiệu lực của Cookie
                });
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
            app.UseStaticFiles(
                new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(@"F:\lap_trinh_web\SV21T1020123\SV21T1020123.Web\wwwroot\images\products\"),
                    RequestPath = "/images"
                });
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            ApplicationContext.Configure(
                context: app.Services.GetRequiredService<IHttpContextAccessor>(),
                enviroment: app.Services.GetRequiredService<IWebHostEnvironment>());

            string ? connectionString = builder.Configuration.GetConnectionString("LiteCommerceDB");
            SV21T1020123.BusinessLayers.Configuration.Initialize(connectionString ?? "");
            app.Run();
        }
    }
}
