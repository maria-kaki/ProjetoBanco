namespace ProjetoBanco.Api.DTOs;

public record CriarAgenciaDto(string Nome, string Numero, string Endereco);
public record AgenciaResponseDto(int Id, string Nome, string Numero, string Endereco);