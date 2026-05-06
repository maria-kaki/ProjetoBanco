namespace ProjetoBanco.Api.Models;

public class Emprestimo : Produto
{
    public decimal ValorMaximo { get; set; }
    public decimal TaxaJurosAnual { get; set; }
    public int PrazoMaximoMeses { get; set; }
}