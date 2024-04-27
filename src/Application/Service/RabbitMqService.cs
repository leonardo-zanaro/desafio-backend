using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Application.Service;

public class RabbitMqService : IRabbitMqService
{
    private readonly IConfiguration _configuration;

    public RabbitMqService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IModel Connect()
    {
        var factory = new ConnectionFactory() { HostName = _configuration["rabbitMq:host"] };
        var connection = factory.CreateConnection();
        return connection.CreateModel();
    }
}