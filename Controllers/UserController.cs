using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using projeto.Service;

namespace projeto.Controllers;

public class UserController (UserService userService) : Controller
{
    
    private readonly UserService _userService = userService;

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(string name, string email, string password, string confirmPassword)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            ModelState.AddModelError("name", "Informe seu nome.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            ModelState.AddModelError("email", "Informe seu e-mail.");
        }

        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
        {
            ModelState.AddModelError("password", "A senha deve ter pelo menos 6 caracteres.");
        }

        if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
        {
            ModelState.AddModelError("confirmPassword", "As senhas não coincidem.");
        }

        if (!ModelState.IsValid)
        {
            return View("Index");
        }

        _userService.CreateUser(name, email, password);
        return RedirectToAction("Index", "Login");
    }

}