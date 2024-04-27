using Application.UseCases.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize(Roles = "Admin")]
[Microsoft.AspNetCore.Components.Route("api/[controller]")]
public class NotificationController : MainController
{
    private readonly INotificationUseCase _notificationUseCase;
    private readonly ILogger<NotificationController> _logger;
    public NotificationController(
        INotificationUseCase notificationUseCase,
        ILogger<NotificationController> logger)
    {
        _notificationUseCase = notificationUseCase;
        _logger = logger;
    }


    /// <summary>
    /// Retrieves all notifications.
    /// </summary>
    /// <param name="pageNumber">The page number of the results (optional).</param>
    /// <param name="pageQuantity">The number of results per page (optional).</param>
    /// <returns>
    /// Returns a list of notifications
    /// </returns>
    [HttpGet]
    [Route("notification/all")]
    public IActionResult GetAllNotifications(int? pageNumber = null, int? pageQuantity = null)
    {
        try
        {
            var result = _notificationUseCase.GetAllNotifications(pageNumber, pageQuantity);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex.Message);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Retrieves notifications by order.
    /// </summary>
    /// <param name="orderId">The ID of the order to retrieve notifications for.</param>
    /// <returns>
    /// The collection of notifications.
    /// </returns>
    [HttpGet]
    [Route("notification/order")]
    public IActionResult GetNotificationByOrder(Guid orderId)
    {
        try
        {
            var result = _notificationUseCase.GetNotificationByOrder(orderId);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex.Message);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Consumes notifications.
    /// </summary>
    /// <returns>
    /// Returns an IActionResult.
    /// </returns>
    [HttpGet]
    [Route("notification/consume")]
    public async Task<IActionResult> ConsumeNotifications()
    {
        try
        {
            await _notificationUseCase.ConsumeNotifications();
            return Ok("All notifications have been consumed");
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex.Message);
            return BadRequest(ex.Message);
        }
    }
}