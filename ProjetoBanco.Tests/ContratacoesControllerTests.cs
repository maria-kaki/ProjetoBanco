using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using ProjetoBanco.Api.Controllers;
using ProjetoBanco.Api.Data;
using ProjetoBanco.Api.DTOs;
using ProjetoBanco.Api.Models;
using ProjetoBanco.Api.Services;
using Xunit;

namespace ProjetoBanco.Tests;

public class ContratacoesControllerTests
{
    private AppDbContext CriarDb()
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(opts);
    }

    private static Mock<IRabbitMqService> MockRabbit()
    {
        return new Mock<IRabbitMqService>();
    }

    [Fact]
    public async Task Solicitar_ClienteInexistente_Retorna404()
    {
        var db = CriarDb();
        var rabbit = MockRabbit();
        var ctrl = new ContratacoesController(db, rabbit.Object);

        var result = await ctrl.Solicitar(new SolicitarContratacaoDto(999, 1));
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Solicitar_Valido_Retorna202()
    {
        var db = CriarDb();
        db.Agencias.Add(new Agencia { Id = 1, Nome = "AG01", Numero = "001", Endereco = "Rua A" });
        db.PessoasFisicas.Add(new PessoaFisica { Id = 1, Nome = "A", Email = "a@a.com", Telefone = "1", AgenciaId = 1, Cpf = "12345678901", DataNascimento = DateTime.Today });
        db.Emprestimos.Add(new Emprestimo { Id = 1, Nome = "Emp", Descricao = "Desc", ValorMaximo = 1000, TaxaJurosAnual = 0.1m, PrazoMaximoMeses = 12 });
        await db.SaveChangesAsync();

        var rabbit = MockRabbit();
        var ctrl = new ContratacoesController(db, rabbit.Object);
        var result = await ctrl.Solicitar(new SolicitarContratacaoDto(1, 1));

        Assert.IsType<AcceptedResult>(result);
    }

    [Fact]
    public async Task Consultar_ContratacaoExistente_RetornaStatus()
    {
        var db = CriarDb();
        db.Agencias.Add(new Agencia { Id = 1, Nome = "AG01", Numero = "001", Endereco = "Rua A" });
        db.PessoasFisicas.Add(new PessoaFisica { Id = 1, Nome = "A", Email = "a@a.com", Telefone = "1", AgenciaId = 1, Cpf = "12345678901", DataNascimento = DateTime.Today });
        db.Emprestimos.Add(new Emprestimo { Id = 1, Nome = "Emp", Descricao = "Desc", ValorMaximo = 1000, TaxaJurosAnual = 0.1m, PrazoMaximoMeses = 12 });
        db.Contratacoes.Add(new Contratacao { Id = 1, ClienteId = 1, ProdutoId = 1, Status = ProjetoBanco.Api.Enums.StatusContratacao.Aprovada });
        await db.SaveChangesAsync();

        var rabbit = MockRabbit();
        var ctrl = new ContratacoesController(db, rabbit.Object);
        var result = await ctrl.Consultar(1);

        Assert.IsType<OkObjectResult>(result);
    }
}