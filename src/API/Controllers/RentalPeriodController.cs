using Application.UseCases.Interfaces;
using Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize(Roles = "Admin")]
[Microsoft.AspNetCore.Components.Route("api/[controller]")]
public class RentalPeriodController : MainController
{
    private readonly IRentalPeriodUseCase _rentalPeriodUseCase;
    private readonly ILogger<RentalPeriodController> _logger;
    public RentalPeriodController(
        IRentalPeriodUseCase rentalPeriodUseCase,
        ILogger<RentalPeriodController> logger)
    {
        _rentalPeriodUseCase = rentalPeriodUseCase;
        _logger = logger;
    }

    [HttpPost]
    [Route("/period/create")]
    public IActionResult Create(RentalPeriodDTO model)
    {
        try
        {
            var result = _rentalPeriodUseCase.Create(model);

            if (!result.Success)
                return BadRequest(result.Message);
            
            return Ok(result.Object);
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex.Message);
            return BadRequest(ex.Message);
        }
    }
    
    [HttpDelete]
    [Route("/period/remove")]
    public IActionResult Remove(Guid rentalPeriodId)
    {
        try
        {
            var result = _rentalPeriodUseCase.Remove(rentalPeriodId);

            if (!result.Success)
                return BadRequest(result.Message);
            
            return Ok(result.Message);
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex.Message);
            return BadRequest(ex.Message);
        }
    }
}