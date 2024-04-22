using Domain.Entities;

namespace Infra.Repositories.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    IEnumerable<Guid> GetAvailableDeliverers();
}