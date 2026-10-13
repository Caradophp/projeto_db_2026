using Microsoft.AspNetCore.Mvc;
using projeto.Models;
using projeto.Service;

namespace projeto.Controllers;

public class TalhaoController(TalhaoService talhaoService) : Controller
{
    private readonly TalhaoService _talhaoService = talhaoService;

    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "Gestão de Talhões";

        return View("~/Views/Shared/ManagementList.cshtml", new ManagementListViewModel
        {
            Title = "Gestão de Talhões",
            Description = "Gerencie as subdivisões (talhões) das propriedades.",
            SearchPlaceholder = "Pesquisar talhão...",
            AddLabel = "Novo Talhão",
            Columns = new[] { "ID", "Nome", "Área", "Propriedade", "Descrição", "Ações" }
        });
    }

    [HttpGet]
    public IActionResult GetList()
    {
        var list = _talhaoService.ListAll();
        return Json(list);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Talhao talhao)
    {
        try
        {
            var success = _talhaoService.Add(talhao);
            return Json(new { success = success, message = success ? "Talhão criado com sucesso!" : "Erro ao criar talhão." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public IActionResult Update([FromBody] Talhao talhao)
    {
        try
        {
            var success = _talhaoService.Update(talhao);
            return Json(new { success = success, message = success ? "Talhão atualizado com sucesso!" : "Erro ao atualizar talhão." });
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
            var success = _talhaoService.Remove(id);
            return Json(new { success = success, message = success ? "Talhão removido com sucesso!" : "Erro ao remover talhão." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}
