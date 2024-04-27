using Domain.Entities;
using Infra.Context;
using Infra.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infra.Repositories;

public class RentalRepository : Repository<Rental>, IRentalRepository
{
    private readonly ILogger<RentalRepository> _logger;
    public RentalRepository(
        DmContext context,
        ILogger<RentalRepository> logger) : base(context, logger)
    {
        _logger = logger;
    }

    public Rental? RentalByMotorcycleId(Guid motorcycleId)
    {
        try
        {
            var rental = _context.Rentals.Where(x =>
                !x.Excluded
                && x.MotorcycleId == motorcycleId
                && x.EndDate == null).OrderByDescending(o => o.StartDate).FirstOrDefault();

            return rental;
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex.Message);
            return null;
        }
    }
}