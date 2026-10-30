using Microsoft.AspNetCore.Mvc;
using projeto.Models;
using projeto.Service;

namespace projeto.Controllers;

public class AplicacaoInsumoController(AplicacaoInsumoService aplicacaoInsumoService) : Controller
{
    private readonly AplicacaoInsumoService _aplicacaoInsumoService = aplicacaoInsumoService;

    [HttpGet]
    public IActionResult Index()
    {
        return View("~/Views/AplicacaoInsumo/Index.cshtml");
    }

    [HttpGet]
    public IActionResult GetList()
    {
        var list = _aplicacaoInsumoService.GetAplicacoes();
        return Json(list);
    }

    [HttpPost]
    public IActionResult Create([FromBody] AplicacaoInsumo aplicacao)
    {
        try
        {
            var success = _aplicacaoInsumoService.RegistrarAplicacao(aplicacao);

            return Json(new
            {
                success,
                message = success
                    ? "Aplicação registrada com sucesso!"
                    : "Erro ao registrar aplicação."
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
    public IActionResult Update([FromBody] AplicacaoInsumo aplicacao)
    {
        try
        {
            if (aplicacao.Quantidade <= 0)
            {
                return Json(new
                {
                    success = false,
                    message = "A quantidade deve ser maior que zero."
                });
            }

            var success = _aplicacaoInsumoService.UpdateAplicacao(aplicacao);

            return Json(new
            {
                success,
                message = success
                    ? "Aplicação atualizada com sucesso!"
                    : "Erro ao atualizar aplicação."
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
            var success = _aplicacaoInsumoService.DeleteAplicacao(id);

            return Json(new
            {
                success,
                message = success
                    ? "Aplicação removida com sucesso!"
                    : "Erro ao remover aplicação."
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