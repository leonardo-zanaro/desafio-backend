using Domain.Entities;

namespace Infra.Repositories.Interfaces;

public interface IMotorcycleRepository : IRepository<Motorcycle>
{
    IEnumerable<Motorcycle> BringAvailables(Guid motorcycleId);
    Motorcycle? GetByPlate(string plate);
}