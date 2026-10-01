using Microsoft.AspNetCore.Mvc;
using projeto.Models;

namespace projeto.Controllers;

public class ColheitaController : Controller
{
    public IActionResult Index()
    {
        ViewData["Title"] = "Colheitas";

        return View("~/Views/Shared/ManagementList.cshtml", new ManagementListViewModel
        {
            Title = "Colheitas",
            Description = "Acompanhe os resultados e registros das colheitas.",
            SearchPlaceholder = "Pesquisar colheita...",
            AddLabel = "Nova colheita",
            Columns = new[] { "Plantio", "Data da colheita", "Produção total", "Observações" }
        });
    }
}