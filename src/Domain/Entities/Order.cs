using Domain.Base;
using Domain.Enums;

namespace Domain.Entities;

public class Order : BaseEntity
{
    public Guid? DelivererId { get; private set; }
    public OrderStatus OrderStatus { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime? DeliveryDate { get; private set; }
    public decimal Price { get; private set; }

    #region Setters
    public Order SetDelivererId(Guid delivererId)
    {
        DelivererId = delivererId;
        OrderStatus = OrderStatus.Accepted;
        
        return this;
    }
    public Order SetCreatedDate()
    {
        CreatedDate = DateTime.UtcNow;
        OrderStatus = OrderStatus.Available;

        return this;
    }
    public Order SetDeliveryDate()
    {
        DeliveryDate = DateTime.UtcNow;
        OrderStatus = OrderStatus.Delivered;
        
        return this;
    }
    public Order SetPrice(decimal price)
    {
        if (price <= 0)
            throw new ArgumentException("The value cannot be negative or zero");
        
        Price = price;
        return this;
    }

    public static Order Create()
    {
        return new Order();
    }
    #endregion
}