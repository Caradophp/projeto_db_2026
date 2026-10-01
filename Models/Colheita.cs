namespace projeto.Models;

public class Colheita
{
    public int Id { get; set; }
    public int PlantioId { get; set; }
    public DateTime DataColheita { get; set; }
    public decimal ProducaoTotal { get; set; }
    public string? Observacoes { get; set; }
}