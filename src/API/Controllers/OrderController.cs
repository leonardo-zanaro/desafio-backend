using Application.UseCases.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class OrderController : MainController
{
    private readonly IOrderUseCase _orderUseCase;

    public OrderController(IOrderUseCase orderUseCase)
    {
        _orderUseCase = orderUseCase;
    }
    
    [HttpPost]
    [Route("create")]
    public IActionResult CreateOrder(decimal price)
    {
        if (price <= 0)
            return BadRequest("Price cannot be 0");
        
        var result = _orderUseCase.CreateOrder(price);
        if (result == null)
            return BadRequest("An unexpected error has occurred");

        return Ok(result);
    }

    [HttpPost]
    [Route("accept")]
    public IActionResult AcceptOrder(Guid orderId, Guid delivererId)
    {
        if (orderId == Guid.Empty || delivererId == Guid.Empty)
            return BadRequest("orderId and delivererId cannot be empty");

        var result = _orderUseCase.AcceptOrder(orderId, delivererId);

        if (!result)
            return BadRequest("An unexpected error has occurred while accepting the order");

        return Ok("Order has been accepted");
    }

    [HttpPost]
    [Route("order-delivery")]
    public IActionResult OrderDelivery(Guid orderId, Guid delivererId)
    {
        if (orderId == Guid.Empty || delivererId == Guid.Empty)
            return BadRequest("orderId and delivererId cannot be empty");

        var result = _orderUseCase.DeliverOrder(orderId);

        if (!result)
            return BadRequest("An unexpected error has occurred while delivering the order");

        return Ok("Order has been delivered");
    }
}