using Application.ViewModel;
using Domain.Entities;

namespace Infra.Repositories.Interfaces;

public interface INotificationRepository : IRepository<Notification>
 {
    IEnumerable<NotificationDTO> GetByOrderId(Guid orderId);
}