using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using projeto.Service;

namespace projeto.Controllers;

public class LoginController (UserService userService) : Controller
{
    
    private readonly UserService _userService = userService;

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public void CheckUserLogin(string email, string password)
    {
        
        if (email.Length == 0)
        {
            throw new Exception("Invalid email");
        }

        if (password.Length == 0)
        {
            throw new Exception("Invalid password");
        }

        bool isValid = _userService.CheckUserLogin(email, password);

        if (!isValid)
        {
            throw new Exception("E-amil ou senha inválidos");
        }

        HttpContext.Response.WriteAsJsonAsync("sucesso");
        //return RedirectToAction("Index", "Home");
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
        bool isValid = _userService.CheckCode(codigo, email);

        if (isValid)
        {
            HttpContext.Response.StatusCode = 200;
            HttpContext.Response.WriteAsJsonAsync("sucesso");
        } else
        {
            HttpContext.Response.StatusCode = 401;
            HttpContext.Response.WriteAsJsonAsync(new ProblemDetails()
            {
                Detail = "Código inválido"
            });
        }
    }

    [HttpPatch]
    public void ChangeUserPassword(string email, string password, string confirmPassword)
    {
        _userService.ChangePass(email, password, confirmPassword);
        HttpContext.Response.StatusCode = 200;
        HttpContext.Response.WriteAsJsonAsync("sucesso");
    }
}