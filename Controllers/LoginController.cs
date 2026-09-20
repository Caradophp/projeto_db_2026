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
    public IActionResult CheckUserLogin(string email, string password)
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
            ModelState.AddModelError("", "E-mail ou senha inválido");
            return View("Index");
        }

        return RedirectToAction("Index", "Home");
    }

}