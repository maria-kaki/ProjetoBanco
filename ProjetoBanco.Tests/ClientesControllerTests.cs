using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoBanco.Api.Controllers;
using ProjetoBanco.Api.Data;
using ProjetoBanco.Api.DTOs;
using ProjetoBanco.Api.Models;
using Xunit;

namespace ProjetoBanco.Tests;

public class ClientesControllerTests
{
    private AppDbContext CriarDb()
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(opts);
    }

    [Fact]
    public async Task CriarPF_Sucesso()
    {
        var db = CriarDb();
        db.Agencias.Add(new Agencia { Id = 1, Nome = "AG01", Numero = "001", Endereco = "Rua A" });
        await db.SaveChangesAsync();

        var ctrl = new ClientesController(db);
        var dto = new CriarPessoaFisicaDto("João", "j@j.com", "11999", 1, "12345678901", DateTime.Today.AddYears(-30));
        var result = await ctrl.CriarPF(dto);

        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task CriarPF_CpfDuplicado_Retorna409()
    {
        var db = CriarDb();
        db.Agencias.Add(new Agencia { Id = 1, Nome = "AG01", Numero = "001", Endereco = "Rua A" });
        db.PessoasFisicas.Add(new PessoaFisica { Nome = "A", Email = "a@a.com", Telefone = "1", AgenciaId = 1, Cpf = "12345678901", DataNascimento = DateTime.Today });
        await db.SaveChangesAsync();

        var ctrl = new ClientesController(db);
        var dto = new CriarPessoaFisicaDto("João", "j@j.com", "11999", 1, "12345678901", DateTime.Today.AddYears(-30));
        var result = await ctrl.CriarPF(dto);

        Assert.IsType<ConflictObjectResult>(result);
    }

    [Fact]
    public async Task CriarPF_AgenciaInexistente_Retorna400()
    {
        var db = CriarDb();
        var ctrl = new ClientesController(db);
        var dto = new CriarPessoaFisicaDto("João", "j@j.com", "11999", 99, "12345678901", DateTime.Today.AddYears(-30));
        var result = await ctrl.CriarPF(dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CriarPJ_CnpjDuplicado_Retorna409()
    {
        var db = CriarDb();
        db.Agencias.Add(new Agencia { Id = 1, Nome = "AG01", Numero = "001", Endereco = "Rua A" });
        db.PessoasJuridicas.Add(new PessoaJuridica { Nome = "Emp", Email = "e@e.com", Telefone = "1", AgenciaId = 1, Cnpj = "12345678000199", RazaoSocial = "Empresa LTDA" });
        await db.SaveChangesAsync();

        var ctrl = new ClientesController(db);
        var dto = new CriarPessoaJuridicaDto("Emp2", "e2@e.com", "1", 1, "12345678000199", "Empresa 2 LTDA");
        var result = await ctrl.CriarPJ(dto);

        Assert.IsType<ConflictObjectResult>(result);
    }
}