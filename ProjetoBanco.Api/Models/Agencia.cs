namespace ProjetoBanco.Api.Models;

public class Agencia
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Numero { get; set; } = null!;
    public string Endereco { get; set; } = null!;

    public ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
}