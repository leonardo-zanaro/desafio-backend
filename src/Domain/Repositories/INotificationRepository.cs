using Domain.Entities;

namespace Infra.Repositories.Interfaces;

public interface INotificationRepository : IRepository<Notification>
 {
    IEnumerable<Notification> GetByOrderId(Guid orderId);
}