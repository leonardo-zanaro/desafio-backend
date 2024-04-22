using Domain.Base;
using Domain.Enums;
using Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Deliverer : BaseEntity
{
    public Guid UserId { get; private set; }
    [Length(14, 14)]
    public string PrimaryDocument { get; private set; }
    [Length(8, 20)]
    public string Cnh { get; private set; }
    public DriversLicense DriverLicense { get; private set; }
    public string? DriverLicenseImage { get; private set; }
    [Length(3, 300)]
    public string Name { get; private set; }
    public DateTime Birthday { get; private set; }
    
    #region Setters
    public Deliverer SetBirthday(DateTime birthday)
    {
        if (DateTime.Today.Year - birthday.Year < 18)
        {
            throw new ArgumentException("The deliverer must be at least 18 years old.");
        }

        Birthday = birthday;
        return this;
    }
    public Deliverer SetName(string name)
    {
        Name = name;
        return this;
    }
    public Deliverer SetPrimaryDocument(CNPJ primaryDocument)
    {
        PrimaryDocument = primaryDocument.Value;
        return this;
    }
    public Deliverer SetCnh(string cnh)
    {
        Cnh = cnh;
        return this;
    }
    public Deliverer SetDriverLicense(DriversLicense driversLicense)
    {
        DriverLicense = driversLicense;
        return this;
    }
    public Deliverer SetDriverLicenseImage(string image)
    {
        DriverLicenseImage = image;
        return this;
    }
    public Deliverer SetUserId(Guid userId)
    {
        UserId = userId;
        return this;
    }
    #endregion

    #region Functions
    public static Deliverer Create()
    {
        var deliverer =  new Deliverer();
        
        return deliverer;
    }
    #endregion
}
