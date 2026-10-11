using Microsoft.AspNetCore.Mvc;
using projeto.Models;
using projeto.Service;

namespace projeto.Controllers;

public class CulturaController(CulturaService culturaService) : Controller
{
    private readonly CulturaService _culturaService = culturaService;

    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "Gestão de Culturas";

        return View("~/Views/Shared/ManagementList.cshtml", new ManagementListViewModel
        {
            Title = "Gestão de Culturas",
            Description = "Gerencie as culturas agrícolas disponíveis.",
            SearchPlaceholder = "Pesquisar cultura...",
            AddLabel = "Nova Cultura",
            Columns = new[] { "ID", "Nome", "Descrição", "Ações" }
        });
    }

    [HttpGet]
    public IActionResult GetList()
    {
        var list = _culturaService.ListAll();
        return Json(list);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Cultura cultura)
    {
        try
        {
            var success = _culturaService.Add(cultura);
            return Json(new { success = success, message = success ? "Cultura criada com sucesso!" : "Erro ao criar cultura." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public IActionResult Update([FromBody] Cultura cultura)
    {
        try
        {
            var success = _culturaService.Update(cultura);
            return Json(new { success = success, message = success ? "Cultura atualizada com sucesso!" : "Erro ao atualizar cultura." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        try
        {
            var success = _culturaService.Remove(id);
            return Json(new { success = success, message = success ? "Cultura removida com sucesso!" : "Erro ao remover cultura." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}
