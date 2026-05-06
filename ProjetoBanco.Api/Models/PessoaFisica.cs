namespace ProjetoBanco.Api.Models;

public class PessoaFisica : Cliente
{
    public string Cpf { get; set; } = null!;
    public DateTime DataNascimento { get; set; }
}