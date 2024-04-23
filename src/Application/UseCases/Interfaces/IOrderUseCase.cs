using Domain.Entities;

namespace Application.UseCases.Interfaces;

public interface IOrderUseCase
{
    Order? CreateOrder(decimal price);
    bool AcceptOrder(Guid orderId, Guid delivererId);
    bool DeliverOrder(Guid orderId);
}