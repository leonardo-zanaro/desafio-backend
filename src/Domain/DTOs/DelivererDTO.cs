using Domain.Enums;

namespace Application.DTOs;

public abstract class UserBase
{
    public string Name { get; set; }
    public string PrimaryDocument { get; set; }
    public DateTime Birthday { get; set; }
    public string Cnh { get; set; }
    public DriversLicense DriversLicense { get; set; }
}

public class GetDelivererDTO : UserBase
{
    public Guid Id { get; set; }
    public string? DriversLicenseImage { get; set; }
}

public class CreateDelivererDTO : UserBase
{
}