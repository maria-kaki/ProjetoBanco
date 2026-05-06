using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace ProjetoBanco.Api.Services;

public class RabbitMqService : IRabbitMqService, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _queueName;

    public RabbitMqService(IConfiguration config)
    {
        _queueName = config["RabbitMQ:Queue"] ?? "contratacoes";
        var factory = new ConnectionFactory
        {
            HostName = config["RabbitMQ:Host"] ?? "localhost"
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(_queueName, durable: true, exclusive: false, autoDelete: false);
    }

    public void PublicarContratacao(int contratacaoId)
    {
        var mensagem = JsonSerializer.Serialize(new { ContratacaoId = contratacaoId });
        var body = Encoding.UTF8.GetBytes(mensagem);
        var props = _channel.CreateBasicProperties();
        props.Persistent = true;
        _channel.BasicPublish("", _queueName, props, body);
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}