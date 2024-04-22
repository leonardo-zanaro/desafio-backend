using Domain.Base;

namespace Domain.Entities;

public class Notification : BaseEntity
{
    public DateTime Date { get; private set; }
    public Guid DelivererId { get; private set; }
    public Guid OrderId { get; private set; }

    #region Setters
    public Notification SetDelivererId(Guid delivererId)
    {
        DelivererId = delivererId;
        return this;
    }
    public Notification SetOrderId(Guid orderId)
    {
        OrderId = orderId;
        return this;
    }
    public static Notification Create()
    {
        return new Notification()
        {
          Date = DateTime.UtcNow
        };
    }
    #endregion
}