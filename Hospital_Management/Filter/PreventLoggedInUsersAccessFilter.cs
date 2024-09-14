using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace Hospital_Management.Filters
{
    public class PreventLoggedInUsersAccessFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                // User is logged in, redirect to homepage with notification
                var routeValues = new RouteValueDictionary(new
                {
                    controller = "Home",
                    action = "Index"
                });

                context.Result = new RedirectToRouteResult(routeValues);
                context.HttpContext.Response.StatusCode = 403; // Optionally set 403 Forbidden status
                context.HttpContext.Items["ErrorMessage"] = "You are already logged in."; // Store message in Items collection
            }
        }
    }
}
