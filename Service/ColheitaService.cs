using projeto.Models;
using projeto.Repository;

namespace projeto.Service;

public class ColheitaService(ColheitaRepository colheitaRepository)
{
    private readonly ColheitaRepository _colheitaRepository = colheitaRepository;

    public IEnumerable<Colheita> GetColheitas() => _colheitaRepository.GetAll();

    public bool AddColheita(Colheita colheita)
    {
        if (colheita.ProducaoTotal < 0) throw new Exception("A produção total não pode ser negativa.");
        return _colheitaRepository.Create(colheita);
    }

    public bool UpdateColheita(Colheita colheita) => _colheitaRepository.Update(colheita);

    public bool DeleteColheita(int id) => _colheitaRepository.Delete(id);
}
