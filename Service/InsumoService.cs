using projeto.Models;
using projeto.Repository;

namespace projeto.Service;

public class InsumoService(InsumoRepository insumoRepository)
{
    private readonly InsumoRepository _insumoRepository = insumoRepository;

    public IEnumerable<Insumo> ListAll()
    {
        return _insumoRepository.GetAll();
    }

    public Insumo? GetById(int id)
    {
        return _insumoRepository.GetById(id);
    }

    public bool Add(Insumo insumo)
    {
        if (insumo.Estoque < 0)
            throw new ArgumentException("O estoque não pode ser negativo.");

        return _insumoRepository.Create(insumo);
    }

    public bool Update(Insumo insumo)
    {
        if (insumo.Estoque < 0)
            throw new ArgumentException("O estoque não pode ser negativo.");

        return _insumoRepository.Update(insumo);
    }

    public bool Remove(int id)
    {
        return _insumoRepository.Delete(id);
    }
}
