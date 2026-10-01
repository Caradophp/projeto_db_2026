namespace projeto.Models;

public class Plantio
{
    public int Id { get; set; }
    public int TalhaoId { get; set; }
    public int CulturaId { get; set; }
    public DateTime DataPlantio { get; set; }
    public DateTime? DataPrevistaColheita { get; set; }
    public decimal? ProdutividadeEstimada { get; set; }
    public decimal? CustoProducao { get; set; }
    public string Status { get; set; } = "Em andamento";
    public string? Observacoes { get; set; }
}