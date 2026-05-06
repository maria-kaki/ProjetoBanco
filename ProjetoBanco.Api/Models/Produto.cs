namespace ProjetoBanco.Api.Models;

public abstract class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Descricao { get; set; } = null!;
}