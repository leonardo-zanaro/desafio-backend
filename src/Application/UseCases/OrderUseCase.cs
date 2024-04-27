using System.Text;
using Application.Service;
using Application.UseCases.Interfaces;
using Application.ViewModel;
using Domain.Entities;
using Infra.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Application.UseCases;

public class OrderUseCase : IOrderUseCase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IDelivererRepository _delivererRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IRabbitMqService _rabbitMqService;
    private readonly ILogger<OrderUseCase> _logger;

    public OrderUseCase(
        IOrderRepository orderRepository,
        IDelivererRepository delivererRepository,
        INotificationRepository notificationRepository, 
        IRabbitMqService rabbitMqService,
        ILogger<OrderUseCase> logger)
    {
        _orderRepository = orderRepository;
        _delivererRepository = delivererRepository;
        _notificationRepository = notificationRepository;
        _rabbitMqService = rabbitMqService;
        _logger = logger;
    }

    public Result CreateOrder(decimal price)
    {
        try
        {
            var order = Order.Create();

            order
                .SetCreatedDate()
                .SetPrice(price);

            var success = _orderRepository.Add(order);

            var deliverers = _orderRepository.GetAvailableDeliverers();

            if (success)
            {
                var channel = _rabbitMqService.Connect();

                channel.QueueDeclare(queue: "notification_queue",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
                foreach (var id in deliverers)
                {
                    var message = JsonConvert.SerializeObject(
                        new OrderNotificationMessage()
                        {
                            OrderId = order.Id,
                            DelivererId = id
                        }
                    );
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(
                        exchange: string.Empty,
                        routingKey: "notification_queue",
                        basicProperties: null,
                        body: body);
                }
            }

            return Result.ObjectResult(order);
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex.Message);
            return Result.FailResult(ex.Message);
        }
    }

    public Result AcceptOrder(Guid orderId, Guid delivererId)
    {
        try
        {
            var deliverer = _delivererRepository.GetById(delivererId);

            if (deliverer == null)
                return Result.FailResult("Deliverer not found.");

            var order = _orderRepository.GetById(orderId);

            if (order == null)
                return Result.FailResult("Order not found.");

            var notification = _notificationRepository.GetByOrderId(orderId);

            if (!(notification.Any(x => x.DelivererId == delivererId)))
                return Result.FailResult("Delivery person has not been notified for this order.");

            order
                .SetDelivererId(delivererId);

            _orderRepository.Update(order);

            return Result.SuccessResult();
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex.Message);
            return Result.FailResult(ex.Message);
        }
    }

    public Result DeliverOrder(Guid orderId)
    {
        try
        {
            var order = _orderRepository.GetById(orderId);
            if (order == null)
                return Result.FailResult("Order not found.");

            order
                .SetDeliveryDate();

            _orderRepository.Update(order);

            return Result.SuccessResult();
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex.Message);
            return Result.FailResult(ex.Message);
        }
    }
}