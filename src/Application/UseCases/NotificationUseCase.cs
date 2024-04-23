using System.Text;
using Domain.Entities;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Application.ViewModel;
using RabbitMQ.Client.Events;
using Infra.Repositories.Interfaces;
using Application.UseCases.Interfaces;
using Infra.Context;
using Microsoft.Extensions.DependencyInjection;

namespace Application.UseCases;

public class NotificationUseCase : INotificationUseCase
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IServiceProvider _serviceProvider;
    public NotificationUseCase(INotificationRepository notificationRepository, IServiceProvider serviceProvider)
    {
        _notificationRepository = notificationRepository;
        _serviceProvider = serviceProvider;
    }

    public IEnumerable<Notification> GetNotificationByOrder(Guid orderId)
    {
        return _notificationRepository.GetByOrderId(orderId);
    }
    
    public async Task ConsumeNotifications()
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
        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var eventMessage = JsonConvert.DeserializeObject<OrderNotificationMessage>(message);

                if (eventMessage != null)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<DmContext>();
                        var notification = StoreNotification(eventMessage);
                        context.Notifications.Add(notification);
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar mensagem: {ex.Message}");
            }
        };
    
        channel.BasicConsume(queue: "notification_queue", autoAck: true, consumer: consumer);

        await Task.Delay(Timeout.Infinite);
    }

    private Notification StoreNotification(OrderNotificationMessage message)
    {
        var notification = Notification.Create();

        notification
            .SetDelivererId(message.DelivererId)
            .SetOrderId(message.OrderId);

       return notification;
    }
}