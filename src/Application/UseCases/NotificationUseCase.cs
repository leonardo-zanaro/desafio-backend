using System.Text;
using Domain.Entities;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Application.ViewModel;
using RabbitMQ.Client.Events;
using Infra.Repositories.Interfaces;
using Application.UseCases.Interfaces;

namespace Application.UseCases;

public class NotificationUseCase : INotificationUseCase
{
    private readonly INotificationRepository _notificationRepository;
    public NotificationUseCase(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public IEnumerable<Notification> GetNotificationByOrder(Guid orderId)
    {
        return _notificationRepository.GetByOrderId(orderId);
    }
    
    public void ConsumeNotifications()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: "notification_queue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var eventMessage = JsonConvert.DeserializeObject<OrderNotificationMessage>(message);
            
            if(eventMessage != null)
                StoreNotification(eventMessage);
        };
        channel.BasicConsume(queue: "notification_queue",
            autoAck: true,
            consumer: consumer);
    }

    private void StoreNotification(OrderNotificationMessage message)
    {
        var notification = Notification.Create();

        notification
            .SetDelivererId(message.DelivererId)
            .SetOrderId(message.OrderId);

        _notificationRepository.Add(notification);
    }
}