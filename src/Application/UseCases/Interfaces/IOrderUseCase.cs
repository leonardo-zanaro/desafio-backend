namespace Application.UseCases.Interfaces;

public interface IOrderUseCase
{
    bool CreateOrder(decimal price);
    bool AcceptOrder(Guid orderId, Guid delivererId);
    bool DeliverOrder(Guid orderId);
}