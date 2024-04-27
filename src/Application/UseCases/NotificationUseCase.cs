using System.Text;
using Application.Service;
using Domain.Entities;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Application.ViewModel;
using RabbitMQ.Client.Events;
using Infra.Repositories.Interfaces;
using Application.UseCases.Interfaces;
using Infra.Context;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Application.UseCases;

public class NotificationUseCase : INotificationUseCase
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IServiceProvider _serviceProvider;
    private readonly IRabbitMqService _rabbitMqService;
    private readonly ILogger<NotificationUseCase> _logger;
    public NotificationUseCase(INotificationRepository notificationRepository, IServiceProvider serviceProvider, IRabbitMqService rabbitMqService, ILogger<NotificationUseCase> logger)
    {
        _notificationRepository = notificationRepository;
        _serviceProvider = serviceProvider;
        _rabbitMqService = rabbitMqService;
        _logger = logger;
    }

    public IEnumerable<NotificationDTO> GetAllNotifications(int? page = null, int? pageQuantity = null)
    {
        try
        {
            var notifications = _notificationRepository.GetAll(page, pageQuantity);

            var list = notifications.Select(notify => new NotificationDTO
            {
                DelivererId = notify.DelivererId,
                OrderId = notify.OrderId,
                Date = notify.Date
            });

            return list;
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex.Message);
            return Enumerable.Empty<NotificationDTO>();
        }
    }

    public IEnumerable<NotificationDTO> GetNotificationByOrder(Guid orderId)
    {
        try
        {
            var notifications = _notificationRepository.GetByOrderId(orderId);
            
            var list = notifications.Select(notify => new NotificationDTO
            {
                DelivererId = notify.DelivererId,
                OrderId = notify.OrderId,
                Date = notify.Date
            });
            
            return list;
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex.Message);
            return Enumerable.Empty<NotificationDTO>();
        }
    }
    
    public Task ConsumeNotifications()
    {
        var channel = _rabbitMqService.Connect();

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
                    using var scope = _serviceProvider.CreateScope();
                    try
                    {
                        var context = scope.ServiceProvider.GetRequiredService<DmContext>();
                        var notification = StoreNotification(eventMessage);
                        context.Notifications.Add(notification);
                        await context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.Log(LogLevel.Error, ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.Message);
            }
        };
    
        channel.BasicConsume(queue: "notification_queue", autoAck: true, consumer: consumer);
        channel.Close();
        channel.Dispose();
        
        return Task.CompletedTask;
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