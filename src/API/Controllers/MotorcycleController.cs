using Application.DTOs;
using Application.UseCases.Interfaces;
using Application.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

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
        _logger = _logger;
    }
    
    [HttpPost]
    [Route("motorcycle/create")]
    public IActionResult CreateMotorcycle(MotorcycleDTO model)
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
            _logger.Log(LogLevel.Error, ex.Message);
            return BadRequest(ex.Message);
        }
    }
    
    [HttpGet]
    [Route("motorcycle/all")]
    public IActionResult GetAll()
    {
        var list = _motorcycleUseCase.GetAll();

        return Ok(list);
    }

    [HttpGet]
    [Route("motorcycle/license-plate")]
    public IActionResult GetByPlate(string plate)
    {
        var resultMotorcycle = _motorcycleUseCase.GetByPlate(plate);

        if (!resultMotorcycle.Success)
            return BadRequest(resultMotorcycle.Message);

        var motorcycle = resultMotorcycle.Object as MotorcycleDTO;
        
        return Ok(motorcycle);
    }


    [HttpPut]
    [Route("motorcycle/update/license-plate")]
    public IActionResult ChangePlate(Guid motorcycleId, string newPlate)
    {
        try
        {
            var resultMotorcycle = _motorcycleUseCase.GetById(motorcycleId);

            if (!resultMotorcycle.Success)
                return BadRequest(resultMotorcycle.Message);

            var motorcycle = resultMotorcycle.Object as MotorcycleDTO;
            
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


    [HttpDelete]
    [Route("motorcycle/remove")]
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