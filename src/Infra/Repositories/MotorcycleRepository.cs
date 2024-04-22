using Domain.Entities;
using Domain.Enums;
using Infra.Context;
using Infra.Repositories.Interfaces;

namespace Infra.Repositories;

public class MotorcycleRepository : Repository<Motorcycle>, IMotorcycleRepository
{
    public MotorcycleRepository(DmContext context) : base(context)
    {
        
    }

    public IEnumerable<Motorcycle> BringAvailables(Guid motorcycleId)
    {
        try
        {
            var list = _context.Motorcycles.Where(x => !x.Excluded && x.Id == motorcycleId && x.Status == StatusMotorcycle.Avaliable);
            return list;
        }
        catch (Exception)
        {
            return Enumerable.Empty<Motorcycle>();
        }
    }
    public Motorcycle? GetByPlate(string plate)
    {
        try
        {
            var motorcycle = _context.Motorcycles.FirstOrDefault(x => !x.Excluded && x.LicensePlate == plate);

            if (motorcycle == null)
                throw new Exception("Motorcycle not found");
            
            return motorcycle;
        }
        catch (Exception)
        {
            return null;
        }
    }
}