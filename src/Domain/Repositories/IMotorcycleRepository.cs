using Domain.Entities;

namespace Infra.Repositories.Interfaces;

public interface IMotorcycleRepository : IRepository<Motorcycle>
{
    Motorcycle? MotorcycleAvaliable(Guid motorcycleId);
    Motorcycle? GetByPlate(string plate);
}