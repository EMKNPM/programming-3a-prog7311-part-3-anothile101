using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Practice_assignment.Filters;

public class AuthorizeFilter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var token = context.HttpContext.Session.GetString("JwtToken");

        // Allow access to Login page without token
        var path = context.HttpContext.Request.Path.ToString().ToLower();
        if (path.Contains("/account/login"))
        {
            return;
        }

        // Redirect to login if no token
        if (string.IsNullOrEmpty(token))
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
        }
    }
}