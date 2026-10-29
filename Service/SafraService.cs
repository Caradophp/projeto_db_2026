using projeto.Models;
using projeto.Repository;

namespace projeto.Service;

public class SafraService(SafraRepository safraRepository)
{
    private readonly SafraRepository _safraRepository = safraRepository;

    public IEnumerable<Safra> ListAll()
    {
        return _safraRepository.GetAll();
    }

    public Safra? GetById(int id)
    {
        return _safraRepository.GetById(id);
    }

    public bool Add(Safra safra)
    {
        if (string.IsNullOrWhiteSpace(safra.Nome))
            throw new ArgumentException("O nome da safra é obrigatório.");

        if (safra.DataFim.HasValue && safra.DataFim < safra.DataInicio)
            throw new ArgumentException("A data de fim não pode ser anterior à data de início.");

        return _safraRepository.Create(safra);
    }

    public bool Update(Safra safra)
    {
        if (string.IsNullOrWhiteSpace(safra.Nome))
            throw new ArgumentException("O nome da safra é obrigatório.");

        if (safra.DataFim.HasValue && safra.DataFim < safra.DataInicio)
            throw new ArgumentException("A data de fim não pode ser anterior à data de início.");

        return _safraRepository.Update(safra);
    }

    public bool Remove(int id)
    {
        return _safraRepository.Delete(id);
    }
}