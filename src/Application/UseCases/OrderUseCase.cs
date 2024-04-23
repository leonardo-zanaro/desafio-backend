using System.Text;
using Application.UseCases.Interfaces;
using Application.ViewModel;
using Domain.Entities;
using Infra.Repositories.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Application.UseCases;

public class OrderUseCase : IOrderUseCase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IDelivererRepository _delivererRepository;
    private readonly INotificationRepository _notificationRepository;

    public OrderUseCase(IOrderRepository orderRepository, IDelivererRepository delivererRepository, INotificationRepository notificationRepository)
    {
        _orderRepository = orderRepository;
        _delivererRepository = delivererRepository;
        _notificationRepository = notificationRepository;
    }

    public Order? CreateOrder(decimal price)
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
                var factory = new ConnectionFactory() { HostName = "localhost" };
                foreach (var id in deliverers)
                {
                    using var connection = factory.CreateConnection();
                    using var channel = connection.CreateModel();
                    
                    channel.QueueDeclare(queue: "notification_queue",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);
                        
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

            return order;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public bool AcceptOrder(Guid orderId, Guid delivererId)
    {
        try
        {
            var deliverer = _delivererRepository.GetById(delivererId);
            
            if (deliverer == null)
                throw new Exception("Deliverer not found.");

            var order = _orderRepository.GetById(orderId);

            if (order == null)
                throw new Exception("Order not found.");

            var notification = _notificationRepository.GetByOrderId(orderId);

            if (!(notification.Any(x => x.DelivererId == delivererId)))
                throw new Exception("Delivery person has not been notified for this order");
            
            order
                .SetDelivererId(delivererId);

            _orderRepository.Update(order);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool DeliverOrder(Guid orderId)
    {
        try
        {
            var order = _orderRepository.GetById(orderId);
            if (order == null)
            {
                throw new Exception("Order not found.");
            }

            order
                .SetDeliveryDate();
            
            var success = _orderRepository.Update(order);

            return success;
        }
        catch (Exception)
        {
            return false;
        }
    }
}