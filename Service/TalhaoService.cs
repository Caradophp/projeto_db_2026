using projeto.Models;
using projeto.Repository;

namespace projeto.Service;

public class TalhaoService(TalhaoRepository talhaoRepository)
{
    private readonly TalhaoRepository _talhaoRepository = talhaoRepository;

    public IEnumerable<Talhao> ListAll()
    {
        return _talhaoRepository.GetAll();
    }

    public Talhao? GetById(int id)
    {
        return _talhaoRepository.GetById(id);
    }

    public bool Add(Talhao talhao)
    {
        if (talhao.Area <= 0)
            throw new ArgumentException("A área do talhão deve ser maior que zero.");

        return _talhaoRepository.Create(talhao);
    }

    public bool Update(Talhao talhao)
    {
        if (talhao.Area <= 0)
            throw new ArgumentException("A área do talhão deve ser maior que zero.");

        return _talhaoRepository.Update(talhao);
    }

    public bool Remove(int id)
    {
        return _talhaoRepository.Delete(id);
    }
}
