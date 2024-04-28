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
    
    /// <summary>
    /// Retrieves all rental periods.
    /// </summary>
    /// <param name="pageNumber">The page number of the results (optional).</param>
    /// <param name="pageQuantity">The number of results per page (optional).</param>
    /// <returns>
    /// The list of rental periods.
    /// </returns>
    [HttpGet]
    [Route("/period")]
    public IActionResult GetAll(int? pageNumber = null, int? pageQuantity = null)
    {
        var list = _rentalPeriodUseCase.GetAll(pageNumber, pageQuantity);

        return Ok(list);
    }

    /// <summary>
    /// Creates a new rental period.
    /// </summary>
    /// <param name="model">The rental period model.</param>
    /// <returns>
    /// The result of the create operation.
    /// </returns>
    [HttpPost]
    [Route("/period")]
    public IActionResult Create(CreateRentalPeriodDTO model)
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

    /// <summary>
    /// Removes a rental period.
    /// </summary>
    /// <param name="rentalPeriodId">The ID of the rental period to remove.</param>
    /// <returns>The result of the remove operation.</returns>
    [HttpDelete]
    [Route("/period")]
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