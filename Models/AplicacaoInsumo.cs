namespace projeto.Models;

public class AplicacaoInsumo
{
    public int Id { get; set; }
    public int TalhaoId { get; set; }
    public int InsumoId { get; set; }
    public DateTime DataAplicacao { get; set; }
    public decimal Quantidade { get; set; }
    public string? Observacoes { get; set; }
}