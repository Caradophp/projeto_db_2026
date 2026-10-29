using Microsoft.AspNetCore.Mvc;
using projeto.Models;
using projeto.Service;

namespace projeto.Controllers;

public class SafraController(SafraService safraService) : Controller
{
    private readonly SafraService _safraService = safraService;

    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "Gestão de Safras";

        return View("~/Views/Shared/ManagementList.cshtml", new ManagementListViewModel
        {
            Title = "Gestão de Safras",
            Description = "Gerencie as safras agrícolas do sistema.",
            SearchPlaceholder = "Pesquisar safra...",
            AddLabel = "Nova Safra",
            Columns = new[] { "ID", "Nome", "Início", "Fim", "Ações" }
        });
    }

    [HttpGet]
    public IActionResult GetList()
    {
        var list = _safraService.ListAll();
        return Json(list);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Safra safra)
    {
        try
        {
            var success = _safraService.Add(safra);
            return Json(new { success = success, message = success ? "Safra criada com sucesso!" : "Erro ao criar safra." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public IActionResult Update([FromBody] Safra safra)
    {
        try
        {
            var success = _safraService.Update(safra);
            return Json(new { success = success, message = success ? "Safra atualizada com sucesso!" : "Erro ao atualizar safra." });
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
            var success = _safraService.Remove(id);
            return Json(new { success = success, message = success ? "Safra removida com sucesso!" : "Erro ao remover safra." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}