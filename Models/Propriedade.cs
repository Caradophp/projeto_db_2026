namespace projeto.Models;

public class Propriedade
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal AreaTotal { get; set; }
    public int UserId { get; set; }
}