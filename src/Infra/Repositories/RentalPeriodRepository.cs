using Domain.Entities;
using Infra.Context;
using Infra.Repositories.Interfaces;

namespace Infra.Repositories;

public class RentalPeriodRepository : Repository<RentalPeriod>, IRentalPeriodRepository
{
    public RentalPeriodRepository(DmContext context) : base(context)
    {
    }
}