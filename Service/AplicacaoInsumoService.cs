using projeto.Models;
using projeto.Repository;

namespace projeto.Service;

public class AplicacaoInsumoService(AplicacaoInsumoRepository repository)
{
    private readonly AplicacaoInsumoRepository _repository = repository;

    public IEnumerable<AplicacaoInsumo> GetAplicacoes() => _repository.GetAll();

    public bool RegistrarAplicacao(AplicacaoInsumo aplicacao)
    {
        if (aplicacao.Quantidade <= 0) throw new Exception("A quantidade deve ser maior que zero.");
        return _repository.Create(aplicacao);
    }

    public bool UpdateAplicacao(AplicacaoInsumo aplicacao) => _repository.Update(aplicacao);

    public bool DeleteAplicacao(int id) => _repository.Delete(id);
}
