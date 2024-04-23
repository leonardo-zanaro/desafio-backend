using Application.DTOs;
using Domain.Entities;
using Domain.Enums;
using Infra.Repositories.Interfaces;

namespace Application.UseCases.Interfaces;

public class MotorcycleUseCase : IMotorcycleUseCase
{
    private readonly IMotorcycleRepository _motorcycleRepository;
    public MotorcycleUseCase(IMotorcycleRepository motorcycleRepository)
    {
        _motorcycleRepository = motorcycleRepository;
    }
    
    public Motorcycle? GetById(Guid motorcycleId)
    {
        var motorcycle = _motorcycleRepository.GetById(motorcycleId);

        if (motorcycle == null)
            return null;

        return motorcycle;
    }

    /// <summary>
    /// Creates a new motorcycle.
    /// </summary>
    /// <param name="model">The motorcycle data to create.</param>
    /// <returns>The created motorcycle.</returns>
    public Motorcycle? CreateMotorcycle(MotorcycleDto model)
    {
        try
        {
            var exist =_motorcycleRepository.GetByPlate(model.LicensePlate);

            if (exist != null)
                throw new Exception("License plate already registered");

            var motorcycle = Motorcycle.Create();
            
            motorcycle
                .SetLicensePlate(model.LicensePlate)
                .SetModel(model.Model)
                .SetYear(model.Year);

            _motorcycleRepository.Add(motorcycle);

            return motorcycle;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public IEnumerable<Motorcycle> GetAll()
    {
        var list = _motorcycleRepository.GetAll();
        return list;
    }

    /// <summary>
    /// Changes the license plate of a motorcycle.
    /// </summary>
    /// <param name="motorcycleId">The ID of the motorcycle.</param>
    /// <param name="plate">The new license plate.</param>
    /// <returns>True if the license plate was successfully changed, false otherwise.</returns>
    public bool ChangePlate(Guid motorcycleId, string plate)
    {
        try
        {
            var motorcycle = _motorcycleRepository.GetById(motorcycleId);

            if (motorcycle == null)
                throw new Exception("Motorcycle not found");

            var motorcycleByLicensePlate = _motorcycleRepository.GetByPlate(plate);

            if (motorcycleByLicensePlate != null)
                throw new Exception("License plate in use");
                        
            motorcycle
                .SetLicensePlate(plate);

            var success =_motorcycleRepository.Update(motorcycle);

            return success;
        }
        catch (Exception)
        {
            return false;
        }
    }
    public Motorcycle? BringAvailable(Guid motorcycleId)
    {
        var list = _motorcycleRepository.BringAvailables(motorcycleId);
        if (!list.Any())
            return null;

        return list.First();
    }
    public Motorcycle? GetByPlate(string plate)
    {
        var motorcycle = _motorcycleRepository.GetByPlate(plate);

        if (motorcycle == null)
            return null;
        
        return motorcycle;
    }

    public bool RemoveMotorcycle(Guid motorcycleId)
    {
        try
        {
            _motorcycleRepository.Remove(motorcycleId);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}