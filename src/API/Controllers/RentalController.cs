using Application.DTOs;
using Application.UseCases.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
[Microsoft.AspNetCore.Components.Route("api/[controller]")]
public class RentalController : MainController
{
    private readonly IRentalUseCase _rentalUseCase;
    private readonly IMotorcycleUseCase _motorcycleUseCase;
    private readonly IDelivererUseCase _delivererUseCase;
    private readonly ILogger<RentalController> _logger;
    public RentalController(
        IRentalUseCase rentalUseCase,
        IMotorcycleUseCase motorcycleUseCase,
        IDelivererUseCase delivererUseCase,
        ILogger<RentalController> logger)
    {
        _rentalUseCase = rentalUseCase;
        _motorcycleUseCase = motorcycleUseCase;
        _delivererUseCase = delivererUseCase;
        _logger = logger;
    }

    [HttpPost]
    [Route("/rent")]
    public IActionResult RentMotorcycle(Guid delivererId, Guid motorcycleId, Guid rentalPeriodId)
    {
        try
        {
            var enabled = _delivererUseCase.MotorcycleEnabled(delivererId);

            if (!enabled.Success)
                return BadRequest(enabled.Message);
        
            var resultMotorcycle = _motorcycleUseCase.BringAvailable(motorcycleId);

            if (!resultMotorcycle.Success)
                return BadRequest(resultMotorcycle.Message);

            var motorcycle = resultMotorcycle.Object as MotorcycleDTO;

            var rental = _rentalUseCase.RentMotorcycle(delivererId, motorcycle.Id.Value, rentalPeriodId);

            if (!rental.Success)
                return BadRequest(rental.Message);

            return Ok("Rental success");
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex.Message);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    [Route("/return")]
    public IActionResult ReturnMotorcycle(Guid motorcycleId)
    {
        try
        {
            var resultMotorcycle = _motorcycleUseCase.GetById(motorcycleId);
            
            if (!resultMotorcycle.Success) 
                return BadRequest(resultMotorcycle.Message);

            var resultRent = _rentalUseCase.RentActive(motorcycleId);
            
            if (!resultRent.Success) 
                return BadRequest(resultRent.Message);

            _rentalUseCase.ReturnMotorcycle(motorcycleId);
            
            return Ok("Motorcycle returned successfully");
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex.Message);
            return BadRequest(ex.Message);
        }
    }
}