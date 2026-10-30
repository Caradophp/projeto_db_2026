using projeto.Models;
using projeto.Repository;

namespace projeto.Service;

public class ColheitaService(ColheitaRepository colheitaRepository)
{
    private readonly ColheitaRepository _colheitaRepository = colheitaRepository;

    public IEnumerable<Colheita> ListAll()
    {
        return _colheitaRepository.GetAll();
    }

    public Colheita? GetById(int id)
    {
        return _colheitaRepository.GetById(id);
    }

    public bool Add(Colheita colheita)
    {
        Validate(colheita);

        return _colheitaRepository.Create(colheita);
    }

    public bool Update(Colheita colheita)
    {
        if (colheita.Id <= 0)
            throw new ArgumentException("Colheita inválida.");

        Validate(colheita);

        return _colheitaRepository.Update(colheita);
    }

    public bool Remove(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Colheita inválida.");

        return _colheitaRepository.Delete(id);
    }

    private static void Validate(Colheita colheita)
    {
        if (colheita.PlantioId <= 0)
            throw new ArgumentException("Informe um plantio válido.");

        if (colheita.DataColheita == default)
            throw new ArgumentException("Informe a data da colheita.");

        if (colheita.DataColheita.Date > DateTime.Today)
            throw new ArgumentException("A data da colheita não pode estar no futuro.");

        if (colheita.ProducaoTotal < 0)
            throw new ArgumentException(
                "A produção total não pode ser negativa."
            );

        if (colheita.Observacoes?.Length > 500)
            throw new ArgumentException(
                "As observações devem possuir no máximo 500 caracteres."
            );
    }
}