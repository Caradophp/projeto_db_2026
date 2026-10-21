using Microsoft.AspNetCore.Mvc;
using projeto.Database;
using projeto.Security;
using projeto.Service;

namespace projeto.Controllers;

public class LoginController(
    UserService userService,
    Jwt jwt,
    MigrationExecutor migrationExecutor) : Controller
{
    private readonly UserService _userService = userService;
    private readonly Jwt _jwt = jwt;
    private readonly MigrationExecutor _migrationExecutor = migrationExecutor;

    public IActionResult Index()
    {
        _migrationExecutor.Init();
        return View();
    }

    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public void CheckUserLogin(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new Exception("E-mail deve ser informado.");
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new Exception("Senha deve ser informada.");
        }

        bool isValid = _userService.CheckUserLogin(
            email,
            password
        );

        if (!isValid)
        {
            throw new Exception("E-mail ou senha inválidos.");
        }

        var token = _jwt.GenerateJwtToken(email, "");

        Dictionary<string, string> data = new()
        {
            { "token", token }
        };

        HttpContext.Response.WriteAsJsonAsync(data);
    }

    [HttpPost]
    public void SendEmailForRetrivePassword(string email)
    {
        _userService.SendEmailForRetrivePassword(email);

        HttpContext.Response.WriteAsJsonAsync("sucesso");
    }

    [HttpPost]
    public void CheckCode(int codigo, string email)
    {
        bool isValid = _userService.CheckCode(
            codigo,
            email
        );

        if (isValid)
        {
            HttpContext.Response.StatusCode = 200;
            HttpContext.Response.WriteAsJsonAsync("sucesso");
        }
        else
        {
            HttpContext.Response.StatusCode = 401;

            HttpContext.Response.WriteAsJsonAsync(
                new ProblemDetails()
                {
                    Detail = "Código inválido ou expirado."
                }
            );
        }
    }

    [HttpPatch]
    public void ChangeUserPassword(
        string email,
        string password,
        string confirmPassword,
        int codigo)
    {
        _userService.ChangePass(
            email,
            password,
            confirmPassword,
            codigo
        );

        HttpContext.Response.StatusCode = 200;
        HttpContext.Response.WriteAsJsonAsync("sucesso");
    }
}