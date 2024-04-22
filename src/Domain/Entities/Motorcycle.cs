using System.ComponentModel.DataAnnotations;
using Domain.Base;
using Domain.Enums;

namespace Domain.Entities;

public class Motorcycle : BaseEntity
{
    [Length(4, 4)]
    public string Year { get; private set; }
    [Length(2, 500)]
    public string Model { get; private set; }
    [Length(4, 20)]
    public string LicensePlate { get; private set; }
    public StatusMotorcycle Status { get; private set; }

    #region Setters
    public Motorcycle SetYear(string year)
    {
        Year = year;
        return this;
    }
    public Motorcycle SetModel(string model)
    {
        if (model == null || model?.Length < 2)
            throw new ArgumentException("The minimum number of characters is 2 for the motorcycle model.");
        
        Model = model;
        return this;
    }
    public Motorcycle SetLicensePlate(string plate)
    {
        if (string.IsNullOrWhiteSpace(plate))
            throw new ArgumentException("The license plate number cannot be empty");
        
        LicensePlate = plate;
        return this;
    }
    public Motorcycle SetStatus(StatusMotorcycle status)
    {
        Status = status;
        return this;
    }
    #endregion
    
    public static Motorcycle Create()
    {
        var motorcycle =  new Motorcycle();
        motorcycle.Status = StatusMotorcycle.Avaliable;
        
        return motorcycle;
    }
}