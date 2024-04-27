namespace Application.ViewModel;

public class NotificationDTO
{
    public Guid DelivererId { get; set; }
    public Guid OrderId { get; set; }
    public DateTime Date { get; set; }
}