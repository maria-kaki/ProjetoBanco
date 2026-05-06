namespace ProjetoBanco.Api.DTOs;

public record CriarPessoaFisicaDto(
    string Nome, string Email, string Telefone,
    int AgenciaId, string Cpf, DateTime DataNascimento);

public record CriarPessoaJuridicaDto(
    string Nome, string Email, string Telefone,
    int AgenciaId, string Cnpj, string RazaoSocial);

public record ClienteResponseDto(
    int Id, string Nome, string Email, string Telefone,
    string Tipo, string Documento, int AgenciaId);