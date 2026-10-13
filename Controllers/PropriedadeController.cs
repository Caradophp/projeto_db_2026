using Microsoft.AspNetCore.Mvc;
using projeto.Models;
using projeto.Service;

namespace projeto.Controllers;

public class PropriedadeController(PropriedadeService propriedadeService) : Controller
{
    private readonly PropriedadeService _propriedadeService = propriedadeService;

    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "Gestão de Propriedades";

        return View("~/Views/Shared/ManagementList.cshtml", new ManagementListViewModel
        {
            Title = "Gestão de Propriedades",
            Description = "Gerencie as propriedades rurais vinculadas ao sistema.",
            SearchPlaceholder = "Pesquisar propriedade...",
            AddLabel = "Nova Propriedade",
            Columns = new[] { "ID", "Nome", "Área Total", "Usuário", "Ações" }
        });
    }

    [HttpGet]
    public IActionResult GetList()
    {
        var list = _propriedadeService.ListAll();
        return Json(list);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Propriedade prop)
    {
        try
        {
            var success = _propriedadeService.Add(prop);
            return Json(new { success = success, message = success ? "Propriedade criada com sucesso!" : "Erro ao criar propriedade." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public IActionResult Update([FromBody] Propriedade prop)
    {
        try
        {
            var success = _propriedadeService.Update(prop);
            return Json(new { success = success, message = success ? "Propriedade atualizada com sucesso!" : "Erro ao atualizar propriedade." });
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
            var success = _propriedadeService.Remove(id);
            return Json(new { success = success, message = success ? "Propriedade removida com sucesso!" : "Erro ao remover propriedade." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}
