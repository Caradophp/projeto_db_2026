using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using projeto.Models;
using projeto.Security;

namespace projeto.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly Jwt _jwt;

    public HomeController(ILogger<HomeController> logger, Jwt jwt)
    {
        _jwt = jwt;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet]
    public void CheckToken(string token)
    {
        bool tokenIsValid = _jwt.ValidateJwtToken(token);

        if (tokenIsValid)
        {
            HttpContext.Response.StatusCode = 200;
            HttpContext.Response.WriteAsJsonAsync("sucesso");
        } else
        {
            HttpContext.Response.StatusCode = 401;
        }
    }
}
