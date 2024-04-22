using Domain.Entities;
using Infra.Context;
using Infra.Repositories.Interfaces;

namespace Infra.Repositories;

public class NotificationRepository : Repository<Notification>, INotificationRepository
{
    public NotificationRepository(DmContext context) : base(context)
    {
    }

    public IEnumerable<Notification> GetByOrderId(Guid orderId)
    {
        try
        {
            var list = _context.Notifications.Where(x => !x.Excluded && x.OrderId == orderId);
            return list;
        }
        catch (Exception)
        {
            return Enumerable.Empty<Notification>();
        }
    }
}