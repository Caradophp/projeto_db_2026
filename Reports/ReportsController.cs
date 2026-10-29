using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using projeto.Database;
using projeto.Security;

namespace projeto.Reports;

[Route("Reports")]
public sealed class ReportsController(
    IConfiguration configuration,
    Jwt jwt,
    IWebHostEnvironment environment,
    MigrationExecutor migrationExecutor) : Controller
{
    private readonly ReportService _reportService = new(configuration);
    private readonly MigrationExecutor _migrationExecutor = migrationExecutor;

    [HttpGet]
    public IActionResult Index()
    {
        var viewerPath = Path.Combine(environment.ContentRootPath, "Reports", "Viewer.html");
        if (!System.IO.File.Exists(viewerPath))
        {
            throw new FileNotFoundException("O visualizador dos relatórios não foi encontrado.");
        }

        _migrationExecutor.Init();
        return View("Index");
    }

    [HttpGet("viewer")]
    public IActionResult Viewer()
    {
        var viewerPath = Path.Combine(environment.ContentRootPath, "Reports", "Viewer.html");
        if (!System.IO.File.Exists(viewerPath))
        {
            throw new FileNotFoundException("O visualizador dos relatórios não foi encontrado.");
        }

        return PhysicalFile(viewerPath, "text/html; charset=utf-8");
    }

    [HttpGet("filters")]
    public IActionResult GetFilters()
    {
        return Ok(_reportService.GetFilterOptions(GetAuthenticatedEmail()));
    }

    [HttpGet("data/{reportId}")]
    public IActionResult GetReport(
        string reportId,
        [FromQuery] ReportFilters filters)
    {
        return Ok(_reportService.GetReport(
            reportId,
            GetAuthenticatedEmail(),
            filters));
    }

    private string GetAuthenticatedEmail()
    {
        var authorization = Request.Headers["Authorization"].ToString();
        const string bearerPrefix = "Bearer ";

        if (!authorization.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException("Autenticação necessária.");
        }

        var token = authorization[bearerPrefix.Length..].Trim();
        if (string.IsNullOrWhiteSpace(token) || !jwt.ValidateJwtToken(token))
        {
            throw new UnauthorizedAccessException("Token de autenticação inválido.");
        }

        var subject = new JwtSecurityTokenHandler().ReadJwtToken(token).Subject;
        if (string.IsNullOrWhiteSpace(subject))
        {
            throw new UnauthorizedAccessException(
                "O token não contém a identificação do usuário.");
        }

        return subject;
    }
}
