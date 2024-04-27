using Application.UseCases.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

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

    [HttpGet]
    [Route("notification/all")]
    public IActionResult GetAllNotifications(int pageNumber, int pageQuantity)
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