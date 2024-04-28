using System.Globalization;
using Application.DTOs;
using Application.UseCases.Interfaces;
using Domain.Entities;
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

    /// <summary>
    /// Rents a motorcycle for a specific period of time.
    /// </summary>
    /// <param name="delivererId">The ID of the deliverer renting the motorcycle.</param>
    /// <param name="motorcycleId">The ID of the motorcycle to be rented.</param>
    /// <param name="rentalPeriodId">The ID of the rental period for the motorcycle.</param>
    /// <returns>If the rental is successful, it will return OK. Otherwise, it will return a bad request status with an error message.</returns>
    [HttpPost]
    [Route("/rental/rent")]
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

            var motorcycle = resultMotorcycle.Object as GetMotorcycleDTO;

            if (motorcycle == null || motorcycle.Id == null)
                return BadRequest("Motorcycle was not found");

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


    /// <summary>
    /// Returns a rented motorcycle.
    /// </summary>
    /// <param name="motorcycleId">The ID of the motorcycle to return.</param>
    /// <returns>If you are successful in returning the motorcycle, it will return ok.</returns>
    [HttpPost]
    [Route("/rental/return")]
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

            var resultReturn = _rentalUseCase.ReturnMotorcycle(motorcycleId);

            if (resultMotorcycle.Object != null)
            {
                var rental = resultReturn.Object as Rental;

                var cultureInfo = new System.Globalization.CultureInfo("pt-BR");
                var format = string.Format(cultureInfo, "{0:C}", rental?.Fine);

                var result = "Motorcycle returned successfully" + (rental?.Fine > 0 ?  $" and the fine amount was: {format}" : string.Empty); 
                
                return Ok(result);
            }

            return BadRequest(resultReturn.Message);

        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex.Message);
            return BadRequest(ex.Message);
        }
    }
}