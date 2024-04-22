using Application.DTOs;
using Application.UseCases.Interfaces;
using Application.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Microsoft.AspNetCore.Components.Route("api")]
public class MotorcycleController : MainController
{
    private readonly IMotorcycleUseCase _motorcycleUseCase;
    private readonly IRentalUseCase _rentalUseCase;

    public MotorcycleController(IMotorcycleUseCase motorcycleUseCase, IRentalUseCase rentalUseCase)
    {
        _motorcycleUseCase = motorcycleUseCase;
        _rentalUseCase = rentalUseCase;
    }

    /// <summary>
    /// Creates a new motorcycle with the specified details.
    /// </summary>
    /// <param name="model">The MotorcycleDto object containing the details of the motorcycle.</param>
    /// <returns>A IActionResult representing the HTTP response. Returns Ok if the motorcycle is successfully created. Returns BadRequest if an error occurs.</returns>
    [HttpPost]
    [Route("motorcycle/post/create")]
    public IActionResult CreateMotorcycle(MotorcycleDto model)
    {
        try
        {
            var motorcycle = _motorcycleUseCase.CreateMotorcycle(model);

            return Ok("Success when creating a motorcycle");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// Retrieves all motorcycles.
    /// </summary>
    /// <returns>
    /// An IEnumerable of Motorcycle objects representing all the motorcycles.
    /// </returns>
    [HttpGet]
    [Route("motorcycle/get/all")]
    public IActionResult GetAll()
    {
        var list = _motorcycleUseCase.GetAll();

        return Ok(list);
    }

    /// <summary>
    /// Retrieves a motorcycle by its license plate.
    /// </summary>
    /// <param name="plate">The license plate of the motorcycle.</param>
    /// <returns>The Motorcycle object representing the motorcycle with the specified license plate. Returns null if no motorcycle is found.</returns>
    [HttpGet]
    [Route("motorcycle/get/license-plate")]
    public IActionResult GetByPlate(string plate)
    {
        var motorcycle = _motorcycleUseCase.GetByPlate(plate);

        if (motorcycle == null)
            return BadRequest("License Plate not found.");
        
        return Ok(motorcycle);
    }


    /// <summary>
    /// Changes the license plate of a motorcycle identified by its ID.
    /// </summary>
    /// <param name="motorcycleId">The ID of the motorcycle to change the license plate for.</param>
    /// <param name="newPlate">The new license plate value.</param>
    /// <returns>A IActionResult representing the HTTP response. Returns Ok if the license plate is successfully changed. Returns BadRequest if the motorcycle is not found or an unexpected error occurs.</returns>
    [HttpPut]
    [Route("motorcycle/put/license-plate")]
    public IActionResult ChangePlate(Guid motorcycleId, string newPlate)
    {
        var motorcycle = _motorcycleUseCase.GetById(motorcycleId);

        if (motorcycle == null)
            return BadRequest("Motorcycle not found");

        var success =_motorcycleUseCase.ChangePlate(motorcycle.Id, newPlate);

        if (!success)
            return BadRequest("An unexpected error has occurred");

        return Ok("Updated motorcycle");
    }


    /// <summary>
    /// Removes a motorcycle with the specified ID.
    /// </summary>
    /// <param name="motorcycleId">The ID of the motorcycle to remove.</param>
    /// <returns>An IActionResult representing the HTTP response. Returns Ok if the motorcycle is successfully removed. Returns BadRequest if an error occurs, such as if the motorcycle is not found or has an active lease.</returns>
    [HttpDelete]
    [Route("motorcycle/remove")]
    public IActionResult RemoveMotorcycle(Guid motorcycleId)
    {
        var motorcycle = _motorcycleUseCase.GetById(motorcycleId);

        if (motorcycle == null)
            return BadRequest("Motorcycle not found");

        var active =_rentalUseCase.RentActive(motorcycleId);

        if (active)
            return BadRequest("Motorcycle with active lease");

        var removed = _motorcycleUseCase.RemoveMotorcycle(motorcycleId);

        if (removed)
            return Ok("Motorcycle with active lease");

        return BadRequest("An unexpected error has occurred");
    }
}