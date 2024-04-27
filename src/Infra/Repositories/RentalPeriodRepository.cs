using Domain.Entities;
using Infra.Context;
using Infra.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infra.Repositories;

public class RentalPeriodRepository : Repository<RentalPeriod>, IRentalPeriodRepository
{
    private readonly ILogger<RentalPeriodRepository> _logger;
    public RentalPeriodRepository(
        DmContext context,
        ILogger<RentalPeriodRepository> logger) : base(context, logger)
    {
        _logger = logger;
    }
}