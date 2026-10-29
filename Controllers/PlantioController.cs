using Microsoft.AspNetCore.Mvc;
using projeto.Models;
using projeto.Service;

namespace projeto.Controllers;

public class PlantioController(PlantioService plantioService) : Controller
{
    private readonly PlantioService _plantioService = plantioService;

    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "Gestão de Plantios";

        return View("~/Views/Shared/ManagementList.cshtml", new ManagementListViewModel
        {
            Title = "Gestão de Plantios",
            Description = "Gerencie os plantios realizados nos talhões.",
            SearchPlaceholder = "Pesquisar plantio...",
            AddLabel = "Novo Plantio",
            Columns = new[] { "ID", "Talhão", "Cultura", "Data Plantio", "Status", "Ações" }
        });
    }

    [HttpGet]
    public IActionResult GetList()
    {
        var list = _plantioService.ListAll();
        return Json(list);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Plantio plantio)
    {
        try
        {
            var success = _plantioService.Add(plantio);
            return Json(new { success = success, message = success ? "Plantio criado com sucesso!" : "Erro ao criar plantio." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public IActionResult Update([FromBody] Plantio plantio)
    {
        try
        {
            var success = _plantioService.Update(plantio);
            return Json(new { success = success, message = success ? "Plantio atualizado com sucesso!" : "Erro ao atualizar plantio." });
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
            var success = _plantioService.Remove(id);
            return Json(new { success = success, message = success ? "Plantio removido com sucesso!" : "Erro ao remover plantio." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}