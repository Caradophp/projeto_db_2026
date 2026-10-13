using projeto.Models;
using projeto.Repository;

namespace projeto.Service;

public class PropriedadeService(PropriedadeRepository propriedadeRepository)
{
    private readonly PropriedadeRepository _propriedadeRepository = propriedadeRepository;

    public IEnumerable<Propriedade> ListAll()
    {
        return _propriedadeRepository.GetAll();
    }

    public Propriedade? GetById(int id)
    {
        return _propriedadeRepository.GetById(id);
    }

    public bool Add(Propriedade prop)
    {
        if (prop.AreaTotal <= 0)
            throw new ArgumentException("A área total da propriedade deve ser maior que zero.");

        return _propriedadeRepository.Create(prop);
    }

    public bool Update(Propriedade prop)
    {
        if (prop.AreaTotal <= 0)
            throw new ArgumentException("A área total da propriedade deve ser maior que zero.");

        return _propriedadeRepository.Update(prop);
    }

    public bool Remove(int id)
    {
        return _propriedadeRepository.Delete(id);
    }
}
