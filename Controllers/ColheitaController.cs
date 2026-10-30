using Microsoft.AspNetCore.Mvc;
using projeto.Models;
using projeto.Service;

namespace projeto.Controllers;

public class ColheitaController(ColheitaService colheitaService) : Controller
{
    private readonly ColheitaService _colheitaService = colheitaService;

    [HttpGet]
   [HttpGet]
public IActionResult Index()
{
    ViewData["Title"] = "Colheitas";

    return View("~/Views/Colheita/Index.cshtml");
}

    [HttpGet]
    public IActionResult GetList()
    {
        var list = _colheitaService.ListAll();

        return Json(list);
    }

    [HttpGet]
    public IActionResult GetById(int id)
    {
        var colheita = _colheitaService.GetById(id);

        if (colheita is null)
        {
            return NotFound(new
            {
                success = false,
                message = "Colheita não encontrada."
            });
        }

        return Json(colheita);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Colheita colheita)
    {
        try
        {
            var success = _colheitaService.Add(colheita);

            return Json(new
            {
                success,
                message = success
                    ? "Colheita criada com sucesso!"
                    : "Erro ao criar colheita."
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    [HttpPost]
    public IActionResult Update([FromBody] Colheita colheita)
    {
        try
        {
            var success = _colheitaService.Update(colheita);

            return Json(new
            {
                success,
                message = success
                    ? "Colheita atualizada com sucesso!"
                    : "Erro ao atualizar colheita."
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        try
        {
            var success = _colheitaService.Remove(id);

            return Json(new
            {
                success,
                message = success
                    ? "Colheita removida com sucesso!"
                    : "Erro ao remover colheita."
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = ex.Message
            });
        }
    }
}