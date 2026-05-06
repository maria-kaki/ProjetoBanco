namespace ProjetoBanco.Api.Models;

public abstract class Cliente
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Telefone { get; set; } = null!;

    public int AgenciaId { get; set; }
    public Agencia Agencia { get; set; } = null!;

    public ICollection<Contratacao> Contratacoes { get; set; } = new List<Contratacao>();
}