using Domain.Entities;
using Domain.Enums;
using Infra.Context;
using Infra.Repositories.Interfaces;

namespace Infra.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(DmContext context) : base(context)
    {
    }

    public IEnumerable<Guid> GetAvailableDeliverers()
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
}