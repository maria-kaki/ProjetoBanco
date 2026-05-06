using Microsoft.EntityFrameworkCore;
using ProjetoBanco.Api.Models;

namespace ProjetoBanco.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Agencia> Agencias { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<PessoaFisica> PessoasFisicas { get; set; }
    public DbSet<PessoaJuridica> PessoasJuridicas { get; set; }
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Emprestimo> Emprestimos { get; set; }
    public DbSet<Contratacao> Contratacoes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Herança TPH com discriminator
        modelBuilder.Entity<Cliente>()
            .HasDiscriminator<string>("TipoCliente")
            .HasValue<PessoaFisica>("PF")
            .HasValue<PessoaJuridica>("PJ");

        modelBuilder.Entity<Produto>()
            .HasDiscriminator<string>("TipoProduto")
            .HasValue<Emprestimo>("EMPRESTIMO");

        // Índices únicos
        modelBuilder.Entity<PessoaFisica>()
            .HasIndex(p => p.Cpf).IsUnique();

        modelBuilder.Entity<PessoaJuridica>()
            .HasIndex(p => p.Cnpj).IsUnique();

        // Seed de produto
        modelBuilder.Entity<Emprestimo>().HasData(new Emprestimo
        {
            Id = 1,
            Nome = "Empréstimo Pessoal",
            Descricao = "Crédito pessoal com análise de score",
            ValorMaximo = 50000,
            TaxaJurosAnual = 0.18m,
            PrazoMaximoMeses = 60
        });
    }
}