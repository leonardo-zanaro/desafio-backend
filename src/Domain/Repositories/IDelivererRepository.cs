using Domain.Entities;

namespace Infra.Repositories.Interfaces;

public interface IDelivererRepository : IRepository<Deliverer>
{
    Deliverer? GetByPrimaryDocument(string document);
    Deliverer? GetByCnh(string cnh);
}