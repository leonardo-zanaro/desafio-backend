using RabbitMQ.Client;

namespace Application.Service;

public interface IRabbitMqService
{
    IModel Connect();
}