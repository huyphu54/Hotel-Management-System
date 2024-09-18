using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace HotelManagement.Filters
{
    public class Authentication : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Kiểm tra xem session có chứa thông tin người dùng hay không
            var session = context.HttpContext.Session.GetString("TaiKhoan");
            if (string.IsNullOrEmpty(session))
            {
                // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
                context.Result = new RedirectToActionResult("Login", "Access", new { area = "Admin" });
            }
            base.OnActionExecuting(context);
        }
    }
}
