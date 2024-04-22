using System.ComponentModel;

namespace Domain.Enums;

public enum DriversLicense
{
    [Description("A")]
    Motorcycle = 0,
    [Description("B")]
    Car = 1,
    [Description("A + B")]
    MotorcycleCar = 2,
    [Description("None")]
    None = 99,
}