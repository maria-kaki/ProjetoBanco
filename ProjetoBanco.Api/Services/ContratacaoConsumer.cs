using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using ProjetoBanco.Api.Data;
using ProjetoBanco.Api.Enums;
using Microsoft.EntityFrameworkCore;

namespace ProjetoBanco.Api.Services;

public class ContratacaoConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _config;
    private IConnection? _connection;
    private IModel? _channel;

    public ContratacaoConsumer(IServiceScopeFactory scopeFactory, IConfiguration config)
    {
        _scopeFactory = scopeFactory;
        _config = config;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory { HostName = _config["RabbitMQ:Host"] ?? "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        var queueName = _config["RabbitMQ:Queue"] ?? "contratacoes";
        _channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (_, ea) =>
        {
            var body = Encoding.UTF8.GetString(ea.Body.ToArray());
            var msg = JsonSerializer.Deserialize<JsonElement>(body);
            var id = msg.GetProperty("ContratacaoId").GetInt32();

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var contratacao = await db.Contratacoes
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(c => c.Id == id, stoppingToken);

            if (contratacao != null)
            {
                // Regra de negócio: simula análise de crédito
                contratacao.Status = StatusContratacao.Aprovada;
                contratacao.DataProcessamento = DateTime.UtcNow;
                contratacao.Observacao = "Aprovado pela análise de crédito automatizada.";
                await db.SaveChangesAsync(stoppingToken);
            }

            _channel.BasicAck(ea.DeliveryTag, false);
        };

        _channel.BasicConsume(queueName, false, consumer);
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}