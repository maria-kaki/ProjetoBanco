using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoBanco.Api.Data;
using ProjetoBanco.Api.DTOs;
using ProjetoBanco.Api.Models;
using ProjetoBanco.Api.Services;

namespace ProjetoBanco.Api.Controllers;

[ApiController]
[Route("api/contratacoes")]
public class ContratacoesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IRabbitMqService _rabbit;  // ← adicione essa linha

    public ContratacoesController(AppDbContext db, IRabbitMqService rabbit)
    {
        _db = db;
        _rabbit = rabbit;
    }

    [HttpPost]
    public async Task<IActionResult> Solicitar([FromBody] SolicitarContratacaoDto dto)
    {
        var cliente = await _db.Clientes.FindAsync(dto.ClienteId);
        if (cliente == null) return NotFound("Cliente não encontrado.");

        var produto = await _db.Produtos.FindAsync(dto.ProdutoId);
        if (produto == null) return NotFound("Produto não encontrado.");

        var contratacao = new Contratacao
        {
            ClienteId = dto.ClienteId,
            ProdutoId = dto.ProdutoId
        };
        _db.Contratacoes.Add(contratacao);
        await _db.SaveChangesAsync();

        _rabbit.PublicarContratacao(contratacao.Id);

        return Accepted(new ContratacaoResponseDto(
            contratacao.Id, contratacao.ClienteId, contratacao.ProdutoId,
            contratacao.Status.ToString(), contratacao.DataSolicitacao, contratacao.Observacao));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Consultar(int id)
    {
        var c = await _db.Contratacoes.FindAsync(id);
        if (c == null) return NotFound();
        return Ok(new ContratacaoResponseDto(
            c.Id, c.ClienteId, c.ProdutoId,
            c.Status.ToString(), c.DataSolicitacao, c.Observacao));
    }
}