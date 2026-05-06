namespace ProjetoBanco.Api.DTOs;

public record SolicitarContratacaoDto(int ClienteId, int ProdutoId);

public record ContratacaoResponseDto(
    int Id, int ClienteId, int ProdutoId,
    string Status, DateTime DataSolicitacao, string? Observacao);