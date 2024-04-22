using Domain.Enums;

namespace Application.DTOs;

public class DelivererDto
{
    public string Name { get; set; }
    public string PrimaryDocument { get; set; }
    public DateTime Birthday { get; set; }
    public string Cnh { get; set; }
    public DriversLicense DriversLicense { get; set; }
}