using Domain.Entities;
using Infra.Context;
using Infra.Repositories.Interfaces;

namespace Infra.Repositories;

public class RentalRepository : Repository<Rental>, IRentalRepository
{
    public RentalRepository(DmContext context) : base(context)
    {
    }

    public Rental? RentalByMotorcycleId(Guid motorcycleId)
    {
        var rental = _context.Rentals.Where(x =>
            !x.Excluded
            && x.MotorcycleId == motorcycleId
            && x.EndDate == null).OrderByDescending(o => o.StartDate).FirstOrDefault();

        return rental;
    }
}