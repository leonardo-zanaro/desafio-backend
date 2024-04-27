using Domain.Entities;
using Domain.Enums;
using Infra.Context;
using Infra.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infra.Repositories;

public class MotorcycleRepository : Repository<Motorcycle>, IMotorcycleRepository
{
    private readonly ILogger<MotorcycleRepository> _logger;
    public MotorcycleRepository(DmContext context, ILogger<MotorcycleRepository> logger) : base(context, logger)
    {
        _logger = logger;
    }

    public Motorcycle? MotorcycleAvaliable(Guid motorcycleId)
    {
        try
        {
            var motorcycle = _context.Motorcycles.FirstOrDefault(x => !x.Excluded && x.Id == motorcycleId && x.Status == StatusMotorcycle.Avaliable);
            return motorcycle;
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error,ex.Message);
            return null;
        }
    }
    public Motorcycle? GetByPlate(string plate)
    {
        try
        {
            var motorcycle = _context.Motorcycles.FirstOrDefault(x => !x.Excluded && x.LicensePlate == plate);

            if (motorcycle == null)
                return null;
            
            return motorcycle;
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error,ex.Message);
            return null;
        }
    }
}