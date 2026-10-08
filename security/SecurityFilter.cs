using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace projeto.Security;

public class SecurityFilter(Jwt jwt) : IAsyncActionFilter
{
    private readonly Jwt _jwt = jwt;

    // Lista de rotas públicas que não exigem autenticação
    private readonly HashSet<string> _publicPaths = new()
    {
        "/login",
        "/Home/CheckToken"
    };

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var path = context.HttpContext.Request.Path.Value?.ToLowerInvariant() ?? "";

        // Verifica se a rota está na lista de exceções públicas
        // Nota: /user foi removido da lista pública para garantir segurança,
        // a menos que existam endpoints específicos de cadastro.
        if (_publicPaths.Contains(path) || path.StartsWith("/login"))
        {
            await next();
            return;
        }

        // Extrai o token do header 'Authorization' (Padrão: Bearer <token>)
        string authHeader = context.HttpContext.Request.Headers["Authorization"];
        string token = null;

        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            token = authHeader.Substring("Bearer ".Length).Trim();
        }

        if (string.IsNullOrEmpty(token) || !_jwt.ValidateJwtToken(token))
        {
            // Para requisições de API ou AJAX, retorna 401 Unauthorized
            if (context.HttpContext.Request.Headers["Accept"].ToString().Contains("application/json"))
            {
                context.Result = new UnauthorizedResult();
            }
            else
            {
                // Para requisições de página, redireciona para o login
                context.Result = new RedirectResult("/login");
            }
            return;
        }

        await next();
    }
}
