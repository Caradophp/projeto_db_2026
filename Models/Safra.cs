namespace projeto.Models;

public class Safra
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty; // Ex: "Safra 2024/2025"
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public string? Descricao { get; set; }
}