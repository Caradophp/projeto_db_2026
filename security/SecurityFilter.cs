using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace projeto.Security;

public class SecurityFilter(Jwt jwt) : IAsyncActionFilter
{
    private readonly Jwt _jwt = jwt;
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var path = context.HttpContext.Request.Path;

        if (path.StartsWithSegments("/login") || path.StartsWithSegments("/user")  || path.StartsWithSegments("/Home/CheckToken"))
        {
            await next();
            return;
        }

        if (path.ToString().Split('/').Length <= 3)
        {
            await next();
            return;
        }

        var token = context.HttpContext.Request.Headers["X-Api-Key"];

        var tokenIsValid =_jwt.ValidateJwtToken(token);

        if (tokenIsValid)
        {
            await next();
            return;
        }

        context.Result = new RedirectResult("/login");
    }
}