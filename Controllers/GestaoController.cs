using Microsoft.AspNetCore.Mvc;
using projeto.Models;

namespace projeto.Controllers;

public class GestaoController : Controller
{
   public IActionResult Propriedades()
{
    return View("~/Views/Propriedade/Index.cshtml");
}

    public IActionResult Talhoes()
{
    return View("~/Views/Talhao/Index.cshtml");
}

    public IActionResult Culturas()
{
    return View("~/Views/Cultura/Index.cshtml");
}

    public IActionResult Insumos()
{
    return View("~/Views/Insumo/Index.cshtml");
}

   public IActionResult Plantios()
{
    return View("~/Views/Plantio/Index.cshtml");
}
    public IActionResult Aplicacoes() => List(
        "Aplicação de Insumos",
        "Consulte as aplicações realizadas em cada talhão.",
        "aplicação",
        "Nova aplicação",
        "Talhão", "Insumo", "Data da aplicação", "Quantidade", "Observações");

    private ViewResult List(
        string title,
        string description,
        string searchTerm,
        string addLabel,
        params string[] columns)
    {
        ViewData["Title"] = title;

        return View("~/Views/Shared/ManagementList.cshtml", new ManagementListViewModel
        {
            Title = title,
            Description = description,
            SearchPlaceholder = $"Pesquisar {searchTerm}...",
            AddLabel = addLabel,
            Columns = columns
        });
    }
}