using Domain.Enums;

namespace Application.DTOs;

public class DelivererDTO
{
    public string Name { get; set; }
    public string PrimaryDocument { get; set; }
    public DateTime Birthday { get; set; }
    public string Cnh { get; set; }
    public DriversLicense DriversLicense { get; set; }
}