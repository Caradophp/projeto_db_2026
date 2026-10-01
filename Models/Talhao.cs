namespace projeto.Models;

public class Talhao
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Area { get; set; }
    public int PropriedadeId { get; set; }
    public string? Descricao { get; set; }
}