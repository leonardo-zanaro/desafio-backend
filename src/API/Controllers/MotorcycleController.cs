using Application.DTOs;
using Application.UseCases.Interfaces;
using Application.ViewModel;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize(Roles = "Admin")]
[Microsoft.AspNetCore.Components.Route("api/[controller]")]
public class MotorcycleController : MainController
{
    private readonly IMotorcycleUseCase _motorcycleUseCase;
    private readonly IRentalUseCase _rentalUseCase;
    private readonly ILogger<MotorcycleController> _logger;

    public MotorcycleController(
        IMotorcycleUseCase motorcycleUseCase,
        IRentalUseCase rentalUseCase,
        ILogger<MotorcycleController> logger)
    {
        _motorcycleUseCase = motorcycleUseCase;
        _rentalUseCase = rentalUseCase;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new motorcycle.
    /// </summary>
    /// <param name="model">The MotorcycleDTO object containing the details of the motorcycle.</param>
    /// <returns>Returns the Motorcycle object if created successfully</returns>
    [HttpPost]
    [Route("/motorcycle")]
    public IActionResult CreateMotorcycle(CreateMotorcycleDTO model)
    {
        try
        {
            var resultCreate = _motorcycleUseCase.CreateMotorcycle(model);

            if (!resultCreate.Success)
                return BadRequest(resultCreate.Message);
            
            return Ok(resultCreate.Object);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Retrieves all motorcycles.
    /// </summary>
    /// <returns>Returns a list of motorcycles</returns>
    [HttpGet]
    [Route("/motorcycle")]
    public IActionResult GetAll(int? pageNumber = null, int? pageQuantity = null)
    {
        var list = _motorcycleUseCase.GetAll(pageNumber, pageQuantity);

        return Ok(list);
    }

    /// <summary>
    /// Retrieves a motorcycle by its license plate.
    /// </summary>
    /// <param name="plate">The license plate of the motorcycle to retrieve.</param>
    /// <returns>The IActionResult representing the result of the operation.</returns>
    [HttpGet]
    [Route("/motorcycle/license-plate")]
    public IActionResult GetByPlate(string plate)
    {
        var resultMotorcycle = _motorcycleUseCase.GetByPlate(plate);

        if (!resultMotorcycle.Success)
            return BadRequest(resultMotorcycle.Message);

        var motorcycle = resultMotorcycle.Object as GetMotorcycleDTO;
        
        return Ok(motorcycle);
    }


    /// <summary>
    /// Changes the license plate of a motorcycle.
    /// </summary>
    /// <param name="motorcycleId">The ID of the motorcycle.</param>
    /// <param name="newPlate">The new license plate.</param>
    /// <returns>The IActionResult representing the result of the operation.</returns>
    [HttpPut]
    [Route("/motorcycle/license-plate")]
    public IActionResult ChangePlate(Guid motorcycleId, string newPlate)
    {
        try
        {
            var resultMotorcycle = _motorcycleUseCase.GetById(motorcycleId);

            if (!resultMotorcycle.Success)
                return BadRequest(resultMotorcycle.Message);

            var motorcycle = resultMotorcycle.Object as GetMotorcycleDTO;
            
            var resultChangePlate =_motorcycleUseCase.ChangePlate(motorcycle.Id.Value, newPlate);

            if (!resultChangePlate.Success)
                return BadRequest(resultChangePlate.Message);

            return Ok("Updated motorcycle");
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex.Message);
            return BadRequest(ex.Message);
        }
    }


    /// <summary>
    /// Removes a motorcycle from the system.
    /// </summary>
    /// <param name="motorcycleId">The unique identifier of the motorcycle to be removed.</param>
    /// <returns>The IActionResult representing the result of the operation.</returns>
    [HttpDelete]
    [Route("/motorcycle")]
    public IActionResult RemoveMotorcycle(Guid motorcycleId)
    {
        try
        {
            var resultMotorcycle = _motorcycleUseCase.GetById(motorcycleId);

            if (!resultMotorcycle.Success)
                return BadRequest(resultMotorcycle.Message);

            var resultActive =_rentalUseCase.RentActive(motorcycleId);

            if (!resultActive.Success)
                return BadRequest(resultActive.Message);

            var rental = resultActive.Object as Rental;

            if (rental != null && rental.EndDate == null)
                return BadRequest("Motorcycle in use.");

            var removed = _motorcycleUseCase.RemoveMotorcycle(motorcycleId);

            if (removed.Success)
                return Ok();

            return BadRequest(removed.Message);
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex.Message);
            return BadRequest(ex.Message);
        }
    }
}