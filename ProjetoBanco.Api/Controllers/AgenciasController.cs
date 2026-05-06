using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoBanco.Api.Data;
using ProjetoBanco.Api.DTOs;
using ProjetoBanco.Api.Models;

namespace ProjetoBanco.Api.Controllers;

[ApiController]
[Route("api/agencias")]
public class AgenciasController : ControllerBase
{
    private readonly AppDbContext _db;
    public AgenciasController(AppDbContext db) => _db = db;

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarAgenciaDto dto)
    {
        var agencia = new Agencia
        {
            Nome = dto.Nome,
            Numero = dto.Numero,
            Endereco = dto.Endereco
        };
        _db.Agencias.Add(agencia);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Buscar), new { id = agencia.Id },
            new AgenciaResponseDto(agencia.Id, agencia.Nome, agencia.Numero, agencia.Endereco));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Buscar(int id)
    {
        var agencia = await _db.Agencias.FindAsync(id);
        if (agencia == null) return NotFound();
        return Ok(new AgenciaResponseDto(agencia.Id, agencia.Nome, agencia.Numero, agencia.Endereco));
    }
}