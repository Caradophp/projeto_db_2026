namespace projeto.Models;

public class Insumo
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Tipo { get; set; }
    public string UnidadeMedida { get; set; } = string.Empty;
    public decimal Estoque { get; set; }
}