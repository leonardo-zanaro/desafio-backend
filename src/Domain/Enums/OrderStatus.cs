using System.ComponentModel;

namespace Domain.Enums;

public enum OrderStatus
{
    [Description("Avaliable")]
    Available = 0,
    [Description("Accepted")]
    Accepted = 1,
    [Description("Delivered")]
    Delivered = 2
}