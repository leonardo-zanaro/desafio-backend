using Application.ViewModel;

namespace Application.UseCases.Interfaces;

public interface INotificationUseCase
{
    IEnumerable<NotificationDTO> GetAllNotifications(int? page = null, int?  pageQuantity = null);
    IEnumerable<NotificationDTO> GetNotificationByOrder(Guid orderId);
    Task ConsumeNotifications();
}