using Domain.Entities;

namespace Application.UseCases.Interfaces;

public interface INotificationUseCase
{
    IEnumerable<Notification> GetNotificationByOrder(Guid orderId);
    Task ConsumeNotifications();
}