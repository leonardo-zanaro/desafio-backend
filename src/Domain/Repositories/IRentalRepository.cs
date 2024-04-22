using Domain.Entities;

namespace Infra.Repositories.Interfaces;

public interface IRentalRepository: IRepository<Rental>
{
    Rental? RentalByMotorcycleId(Guid motorcycleId);
}