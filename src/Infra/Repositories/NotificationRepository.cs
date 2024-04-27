using Application.ViewModel;
using Domain.Entities;
using Infra.Context;
using Infra.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infra.Repositories;

public class NotificationRepository : Repository<Notification>, INotificationRepository
{
    private readonly ILogger<NotificationRepository> _logger;
    public NotificationRepository(
        DmContext context,
        ILogger<NotificationRepository> logger) : base(context, logger)
    {
        _logger = logger;
    }

    public IEnumerable<NotificationDTO> GetByOrderId(Guid orderId)
    {
        try
        {
            var list = _context.Notifications.Where(x => !x.Excluded && x.OrderId == orderId).Select(notify => new NotificationDTO
            {
                DelivererId = notify.DelivererId,
                OrderId = notify.OrderId,
                Date = notify.Date
            });
            return list;
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex.Message);
            return Enumerable.Empty<NotificationDTO>();
        }
    }
}