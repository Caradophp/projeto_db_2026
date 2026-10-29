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

    public IActionResult Culturas() => List(
        "Culturas",
        "Consulte as culturas utilizadas nos plantios.",
        "cultura",
        "Nova cultura",
        "Nome", "Descrição");

    public IActionResult Insumos() => List(
        "Insumos",
        "Acompanhe os insumos disponíveis e seus níveis de estoque.",
        "insumo",
        "Novo insumo",
        "Nome", "Tipo", "Unidade de medida", "Estoque");

    public IActionResult Plantios() => List(
        "Plantios",
        "Acompanhe culturas, datas e situação dos plantios.",
        "plantio",
        "Novo plantio",
        "Talhão", "Cultura", "Data de plantio", "Colheita prevista", "Produtividade estimada", "Status");

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