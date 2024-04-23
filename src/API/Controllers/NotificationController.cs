using Application.UseCases.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Microsoft.AspNetCore.Components.Route("api/[controller]")]
public class NotificationController : MainController
{
    private readonly INotificationUseCase _notificationUseCase;
    public NotificationController(INotificationUseCase notificationUseCase)
    {
        _notificationUseCase = notificationUseCase;
    }

    [HttpGet]
    [Route("notifications")]
    public IActionResult GetNotificationByOrder(Guid orderId)
    {
        var result = _notificationUseCase.GetNotificationByOrder(orderId);
        
        return Ok(result);
    }
    
    [HttpGet]
    [Route("consume")]
    public async Task<IActionResult> ConsumeNotifications()
    {
        await _notificationUseCase.ConsumeNotifications();
        return Ok("All notifications have been consumed");
    }
}