using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using projeto.Models;
using projeto.Security;
using projeto.Service;

namespace projeto.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly Jwt _jwt;

    private readonly HomeService _homeService;

    public HomeController(ILogger<HomeController> logger, Jwt jwt, HomeService homeService)
    {
        _jwt = jwt;
        _logger = logger;
        _homeService = homeService;
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
public IActionResult CheckToken()
{
    var authorizationHeader = Request.Headers.Authorization.ToString();

    if (string.IsNullOrWhiteSpace(authorizationHeader) ||
        !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
    {
        return Unauthorized();
    }

    var token = authorizationHeader["Bearer ".Length..].Trim();

    if (string.IsNullOrWhiteSpace(token) || !_jwt.ValidateJwtToken(token))
    {
        return Unauthorized();
    }

    return Ok(new { success = true });
}

    [HttpGet]
    public IActionResult GetRegisterNumbersPerTable()
    {
        return Json(_homeService.GetRegisterNumbersPerTable());
    }
}
