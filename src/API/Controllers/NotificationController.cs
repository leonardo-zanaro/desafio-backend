using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Microsoft.AspNetCore.Components.Route("api/[controller]")]
public class NotificationController : MainController
{
    public NotificationController()
    {
        
    }

    [HttpGet]
    [Route("get")]
    public IActionResult GetNotificationByOrder(Guid orderId)
    {
        return Ok();
    }
}