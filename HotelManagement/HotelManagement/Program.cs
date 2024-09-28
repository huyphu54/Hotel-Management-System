using DinkToPdf.Contracts;
using DinkToPdf;
using HotelManagement.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using HotelManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<QlksContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("QLKS")));
// Đăng ký dịch vụ IConverter
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
//Thêm Session
builder.Services.AddSession();
// Đăng ký dịch vụ EmailService
builder.Services.AddTransient<EmailService>();

// Cấu hình xác thực và phân quyền
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Login";
        options.LogoutPath = "/Admin/Logout";
    });

//builder.Services.AddAuthorization(options =>
//{
//    // Thiết lập phân quyền cho khu vực Admin
//    options.AddPolicy("AdminOnly", policy => policy.RequireAuthenticatedUser());
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Sử dụng Session
app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Xác thực và phân quyền
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapAreaControllerRoute(
    name: "admin",
    areaName: "Admin",
    pattern: "Admin/{controller=Access}/{action=Login}/{id?}")
    .RequireAuthorization("AdminOnly");  // Áp dụng chính sách phân quyền cho khu vực Admin

app.Run();  
