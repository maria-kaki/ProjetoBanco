namespace ProjetoBanco.Api.Models;

public class PessoaJuridica : Cliente
{
    public string Cnpj { get; set; } = null!;
    public string RazaoSocial { get; set; } = null!;
}