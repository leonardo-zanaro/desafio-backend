using Application.ViewModel;
using Domain.Entities;

namespace Application.UseCases.Interfaces;

public interface INotificationUseCase
{
    IEnumerable<NotificationDTO> GetAllNotifications(int page, int pageQuantity);
    IEnumerable<NotificationDTO> GetNotificationByOrder(Guid orderId);
    Task ConsumeNotifications();
}