using Domain.Entities;
using Domain.Enums;
using Infra.Context;
using Infra.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infra.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    private readonly ILogger<OrderRepository> _logger;
    public OrderRepository(
        DmContext context,
        ILogger<OrderRepository> logger) : base(context, logger)
    {
        _logger = logger;
    }

    public IEnumerable<Guid> GetAvailableDeliverers()
    {
        try
        {
            var activeDeliverers = _context.Rentals
                .Where(x => !x.Excluded && x.EndDate == null)
                .Select(s => s.DelivererId)
                .Distinct();

            var deliverersWithUndeliveredOrders = _context.Orders
                .Where(x => x.OrderStatus != OrderStatus.Delivered && x.DelivererId != null)
                .Select(s => s.DelivererId.Value)
                .Distinct();

            return activeDeliverers.Except(deliverersWithUndeliveredOrders);
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex.Message);
            return Enumerable.Empty<Guid>();
        }
    }
}