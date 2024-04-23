using Application.UseCases.Interfaces;
using Application.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class RentalController : MainController
{
    private readonly IRentalUseCase _rentalUseCase;
    private readonly IMotorcycleUseCase _motorcycleUseCase;
    private readonly IDelivererUseCase _delivererUseCase;
    public RentalController(IRentalUseCase rentalUseCase, IMotorcycleUseCase motorcycleUseCase, IDelivererUseCase delivererUseCase)
    {
        _rentalUseCase = rentalUseCase;
        _motorcycleUseCase = motorcycleUseCase;
        _delivererUseCase = delivererUseCase;
    }


    /// <summary>
    /// Rents a motorcycle to a deliverer for a specific rental period.
    /// </summary>
    /// <param name="delivererId">The ID of the deliverer who is renting the motorcycle.</param>
    /// <param name="motorcycleId">The ID of the motorcycle being rented.</param>
    /// <param name="rentalPeriodId">The ID of the rental period.</param>
    /// <param name="endDate">The end date of the rental period.</param>
    /// <returns>Returns an IActionResult indicating the result of the operation.</returns>
    [HttpPost]
    [Route("rental/rent")]
    public IActionResult RentMotorcycle(Guid delivererId, Guid motorcycleId, Guid rentalPeriodId)
    {
        try
        {
            var enabled = _delivererUseCase.MotorcycleEnabled(delivererId);
        
            if (!enabled)
                throw new Exception("Unlicensed driver");
        
            var motorcycle = _motorcycleUseCase.BringAvailable(motorcycleId);

            if (motorcycle == null)
                throw new Exception("Motorcycle not available");

            var rental = _rentalUseCase.RentMotorcycle(delivererId, motorcycle.Id, rentalPeriodId);

            if (!rental)
                throw new Exception("An unexpected error has occurred");

            return Ok("Rental success");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// Returns a motorcycle.
    /// </summary>
    /// <param name="motorcycleId">The ID of the motorcycle to return.</param>
    /// <returns>Returns an IActionResult indicating the result of the operation.</returns>
    [HttpPost]
    [Route("rental/return")]
    public IActionResult ReturnMotorcycle(Guid motorcycleId)
    {
        try
        {
            var motorcycle = _motorcycleUseCase.GetById(motorcycleId);
            if (motorcycle == null) throw new Exception("Motorcycle not found");

            var success = _rentalUseCase.RentActive(motorcycleId);
            if (!success) throw new Exception("An unexpected error has occurred");

            _rentalUseCase.ReturnMotorcycle(motorcycleId);
            
            return Ok("Motorcycle returned successfully");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}