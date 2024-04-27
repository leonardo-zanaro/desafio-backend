using Application.DTOs;
using Application.UseCases.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
[Microsoft.AspNetCore.Components.Route("api/[controller]")]
public class OrderController : MainController
{
    private readonly IOrderUseCase _orderUseCase;
    private readonly ILogger<OrderController> _logger;
    public OrderController(
        IOrderUseCase orderUseCase,
        ILogger<OrderController> logger)
    {
        _orderUseCase = orderUseCase;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new order with the specified price.
    /// </summary>
    /// <param name="price">The price of the order.</param>
    /// <returns>Returns an IActionResult representing the result of the operation.</returns>
    [HttpPost]
    [Route("order/create")]
    [Authorize(Roles = "Admin")]
    public IActionResult CreateOrder(decimal price)
    {
        try
        {
            if (price <= 0)
                return BadRequest("Price cannot be 0");
            
            var result = _orderUseCase.CreateOrder(price);
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
    /// Accepts an order with the specified order and deliverer IDs.
    /// </summary>
    /// <param name="orderId">The ID of the order to accept.</param>
    /// <param name="delivererId">The ID of the deliverer accepting the order.</param>
    /// <returns>Returns an IActionResult representing the result of the operation.</returns>
    [HttpPost]
    [Route("order/accept")]
    public IActionResult AcceptOrder(Guid orderId, Guid delivererId)
    {
        try
        {
            if (orderId == Guid.Empty || delivererId == Guid.Empty)
                return BadRequest("orderId and delivererId cannot be empty");

            var result = _orderUseCase.AcceptOrder(orderId, delivererId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok("Order has been accepted");
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex.Message);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Delivers an order with the specified order and deliverer IDs.
    /// </summary>
    /// <param name="orderId">The ID of the order to deliver.</param>
    /// <param name="delivererId">The ID of the deliverer delivering the order.</param>
    /// <returns>Returns an IActionResult representing the result of the operation.</returns>
    [HttpPost]
    [Route("order/delivery")]
    public IActionResult OrderDelivery(Guid orderId, Guid delivererId)
    {
        try
        {
            if (orderId == Guid.Empty || delivererId == Guid.Empty)
                return BadRequest("orderId and delivererId cannot be empty");

            var result = _orderUseCase.DeliverOrder(orderId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok("Order has been delivered");
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex.Message);
            return BadRequest(ex.Message);
        }
    }
}