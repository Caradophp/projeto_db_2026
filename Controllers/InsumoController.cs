using Microsoft.AspNetCore.Mvc;
using projeto.Models;
using projeto.Service;

namespace projeto.Controllers;

public class InsumoController(InsumoService insumoService) : Controller
{
    private readonly InsumoService _insumoService = insumoService;

    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "Gestão de Insumos";

        return View("~/Views/Shared/ManagementList.cshtml", new ManagementListViewModel
        {
            Title = "Gestão de Insumos",
            Description = "Gerencie os insumos utilizados na lavoura.",
            SearchPlaceholder = "Pesquisar insumo...",
            AddLabel = "Novo Insumo",
            Columns = new[] { "ID", "Nome", "Tipo", "Unidade", "Estoque", "Ações" }
        });
    }

    [HttpGet]
    public IActionResult GetList()
    {
        var list = _insumoService.ListAll();
        return Json(list);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Insumo insumo)
    {
        try
        {
            var success = _insumoService.Add(insumo);
            return Json(new { success = success, message = success ? "Insumo criado com sucesso!" : "Erro ao criar insumo." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public IActionResult Update([FromBody] Insumo insumo)
    {
        try
        {
            var success = _insumoService.Update(insumo);
            return Json(new { success = success, message = success ? "Insumo atualizado com sucesso!" : "Erro ao atualizar insumo." });
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
            var success = _insumoService.Remove(id);
            return Json(new { success = success, message = success ? "Insumo removido com sucesso!" : "Erro ao remover insumo." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}
