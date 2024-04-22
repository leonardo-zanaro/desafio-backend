using System.ComponentModel;

namespace Domain.Enums;

public enum StatusMotorcycle
{
    [Description("Avaliable")]
    Avaliable = 0,
    [Description("Reserved")]
    Reserved = 1,
    [Description("Useless")]
    Useless = 2,
}