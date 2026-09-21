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
    public void Create(string name, string email, string password, string confirmPassword)
    {
        _userService.CreateUser(name, email, password, confirmPassword);
        HttpContext.Response.WriteAsJsonAsync("sucesso");
        //return RedirectToAction("Index", "Login");
    }

}