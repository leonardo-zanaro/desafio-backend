using Application.ViewModel;

namespace Application.UseCases.Interfaces;

public interface IOrderUseCase
{
    Result CreateOrder(decimal price);
    Result AcceptOrder(Guid orderId, Guid delivererId);
    Result DeliverOrder(Guid orderId);
}