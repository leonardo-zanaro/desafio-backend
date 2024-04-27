using Application.DTOs;
using Application.UseCases.Interfaces;
using Application.ViewModel;
using Domain.Entities;
using Infra.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.UseCases;

public class MotorcycleUseCase : IMotorcycleUseCase
{
    private readonly IMotorcycleRepository _motorcycleRepository;
    private readonly ILogger<MotorcycleUseCase> _logger;
    public MotorcycleUseCase(IMotorcycleRepository motorcycleRepository, ILogger<MotorcycleUseCase> logger)
    {
        _motorcycleRepository = motorcycleRepository;
        _logger = logger;
    }
    
    public Result GetById(Guid motorcycleId)
    {
        try
        {
            var motorcycle = _motorcycleRepository.GetById(motorcycleId);

            if(motorcycle == null)
                return Result.FailResult("Motorcycle not found.");
            
            var dto = new MotorcycleDTO
            {
                Id = motorcycle.Id,
                LicensePlate = motorcycle.LicensePlate,
                Year = motorcycle.Year,
                Model = motorcycle.Model
            };

            _logger.LogInformation($"Motorcycle with the following ID: {motorcycleId} was found");
            return Result.ObjectResult(dto, "Success when searching for a motorcycle");
        }
        catch (Exception ex)
        {
            _logger.LogError( ex.Message);
            return Result.FailResult(ex.Message);
        }
    }

    public Result CreateMotorcycle(MotorcycleDTO model)
    {
        try
        {
            var exist =_motorcycleRepository.GetByPlate(model.LicensePlate);

            if (exist != null)
                return Result.FailResult("License plate already registered");

            var motorcycle = Motorcycle.Create();
            
            motorcycle
                .SetLicensePlate(model.LicensePlate)
                .SetModel(model.Model)
                .SetYear(model.Year);

            _motorcycleRepository.Add(motorcycle);

            var motorcycleDto = new MotorcycleDTO()
            {
                Id = motorcycle.Id,
                Year = motorcycle.Year,
                Model = motorcycle.Model,
                LicensePlate = motorcycle.LicensePlate
            };

            _logger.LogInformation("Success when creating a motorcycle");
            return Result.ObjectResult(motorcycleDto, "Success when creating a motorcycle");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return Result.FailResult(ex.Message);
        }
    }

    public IEnumerable<MotorcycleDTO> GetAll(int? page = null, int? pageQuantity = null)
    {
        try
        {
            var list = _motorcycleRepository.GetAll(page, pageQuantity).Select(motorcycle => new MotorcycleDTO
            {
                Id = motorcycle.Id,
                LicensePlate = motorcycle.LicensePlate,
                Year = motorcycle.Year,
                Model = motorcycle.Model
            }).ToList();
            
            
            _logger.LogInformation($"Success in bringing {list.Count()} motorcycles");
            return list;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return Enumerable.Empty<MotorcycleDTO>();
        }
    }

    public Result ChangePlate(Guid motorcycleId, string plate)
    {
        try
        {
            var motorcycle = _motorcycleRepository.GetById(motorcycleId);

            if (motorcycle == null)
                return Result.FailResult("Motorcycle not found");

            var motorcycleByLicensePlate = _motorcycleRepository.GetByPlate(plate);

            if (motorcycleByLicensePlate != null)
                return Result.FailResult("License plate in use");
                        
            motorcycle
                .SetLicensePlate(plate);

            var success =_motorcycleRepository.Update(motorcycle);

            if (!success)
                return Result.FailResult("Failed to update motorcycle");

            _logger.LogInformation($"Success when upgrading motorcycle from plate {motorcycle.LicensePlate} to plate {plate}");
            return Result.SuccessResult("Successfully updated motorcycle");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return Result.FailResult(ex.Message);
        }
    }
    public Result BringAvailable(Guid motorcycleId)
    {
        try
        {
            var motorcycle = _motorcycleRepository.MotorcycleAvaliable(motorcycleId);

            if (motorcycle == null)
                return Result.FailResult("Motorcycle not found.");
            
            var dto = new MotorcycleDTO
            {
                Id = motorcycle.Id,
                LicensePlate = motorcycle.LicensePlate,
                Year = motorcycle.Year,
                Model = motorcycle.Model
            };
            
            _logger.LogInformation($"Motorcycle successfully found");
            return Result.ObjectResult(dto);
        }
        catch (Exception ex)
        {  
            _logger.LogError(ex.Message);
            return Result.FailResult(ex.Message);
        }
        
    }
    public Result GetByPlate(string plate)
    {
        try
        {
            var motorcycle = _motorcycleRepository.GetByPlate(plate);

            if (motorcycle == null)
                return Result.FailResult("Motorcycle not found.");

            var motorcycleDto = new MotorcycleDTO()
            {
                Id = motorcycle.Id,
                Year = motorcycle.Year,
                Model = motorcycle.Model,
                LicensePlate = motorcycle.LicensePlate
            };
        
            _logger.LogInformation($"Motorcycle successfully found");
            return Result.ObjectResult(motorcycleDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return Result.FailResult(ex.Message);
        }
    }

    public Result RemoveMotorcycle(Guid motorcycleId)
    {
        try
        {
            _motorcycleRepository.Remove(motorcycleId);
            _logger.LogInformation($"Motorcycle with the following id: {motorcycleId} has been removed");
            return Result.SuccessResult("Success when removing motorcycle");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return Result.FailResult(ex.Message);
        }
    }
}