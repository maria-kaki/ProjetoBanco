namespace ProjetoBanco.Api.Services;

public interface IRabbitMqService
{
    void PublicarContratacao(int contratacaoId);
}