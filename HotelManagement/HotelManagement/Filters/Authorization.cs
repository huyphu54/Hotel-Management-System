using System;
using System.Collections.Generic;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace HotelManagement.Filters
{
    public class Authorization : ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var role = context.HttpContext.Session.GetString("Role");
            if (role == null || role != "Admin")
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
