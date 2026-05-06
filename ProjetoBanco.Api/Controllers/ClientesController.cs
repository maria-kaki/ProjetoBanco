using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoBanco.Api.Data;
using ProjetoBanco.Api.DTOs;
using ProjetoBanco.Api.Models;

namespace ProjetoBanco.Api.Controllers;

[ApiController]
[Route("api/clientes")]
public class ClientesController : ControllerBase
{
    private readonly AppDbContext _db;
    public ClientesController(AppDbContext db) => _db = db;

    [HttpPost("pf")]
    public async Task<IActionResult> CriarPF([FromBody] CriarPessoaFisicaDto dto)
    {
        var agencia = await _db.Agencias.FindAsync(dto.AgenciaId);
        if (agencia == null) return BadRequest("Agência não encontrada.");

        if (await _db.PessoasFisicas.AnyAsync(p => p.Cpf == dto.Cpf))
            return Conflict("CPF já cadastrado.");

        var pf = new PessoaFisica
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Telefone = dto.Telefone,
            AgenciaId = dto.AgenciaId,
            Cpf = dto.Cpf,
            DataNascimento = dto.DataNascimento
        };
        _db.PessoasFisicas.Add(pf);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Buscar), new { id = pf.Id },
            MapResponse(pf));
    }

    [HttpPost("pj")]
    public async Task<IActionResult> CriarPJ([FromBody] CriarPessoaJuridicaDto dto)
    {
        var agencia = await _db.Agencias.FindAsync(dto.AgenciaId);
        if (agencia == null) return BadRequest("Agência não encontrada.");

        if (await _db.PessoasJuridicas.AnyAsync(p => p.Cnpj == dto.Cnpj))
            return Conflict("CNPJ já cadastrado.");

        var pj = new PessoaJuridica
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Telefone = dto.Telefone,
            AgenciaId = dto.AgenciaId,
            Cnpj = dto.Cnpj,
            RazaoSocial = dto.RazaoSocial
        };
        _db.PessoasJuridicas.Add(pj);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Buscar), new { id = pj.Id },
            MapResponse(pj));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Buscar(int id)
    {
        var cliente = await _db.Clientes.FindAsync(id);
        if (cliente == null) return NotFound();
        return Ok(MapResponse(cliente));
    }

    private static ClienteResponseDto MapResponse(Cliente c) => c switch
    {
        PessoaFisica pf => new(pf.Id, pf.Nome, pf.Email, pf.Telefone, "PF", pf.Cpf, pf.AgenciaId),
        PessoaJuridica pj => new(pj.Id, pj.Nome, pj.Email, pj.Telefone, "PJ", pj.Cnpj, pj.AgenciaId),
        _ => throw new InvalidOperationException()
    };
}