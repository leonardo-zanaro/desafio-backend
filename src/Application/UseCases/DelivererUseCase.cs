using Application.DTOs;
using Application.UseCases.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using Infra.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.UseCases;

public class DelivererUseCase : IDelivererUseCase
{
    private readonly IDelivererRepository _delivererRepository;

    public DelivererUseCase(IDelivererRepository delivererRepository)
    {
        _delivererRepository = delivererRepository;
    }

    /// <summary>
    /// Checks if the deliverer is enabled to use a motorcycle.
    /// </summary>
    /// <param name="delivererId">The ID of the deliverer.</param>
    /// <returns>True if the deliverer is enabled to use a motorcycle, false otherwise.</returns>
    public bool MotorcycleEnabled(Guid delivererId)
    {
        var deliverer = _delivererRepository.GetById(delivererId);

        if (deliverer == null)
            return false;
        
        if (deliverer.DriverLicense == DriversLicense.Motorcycle || deliverer.DriverLicense == DriversLicense.MotorcycleCar)
            return true;

        return false;
    }

    /// <summary>
    /// Checks if the deliverer is enabled to use a car.
    /// </summary>
    /// <param name="delivererId">The ID of the deliverer.</param>
    /// <returns>True if the deliverer is enabled to use a car, false otherwise.</returns>
    public bool CarEnabled(Guid delivererId)
    {
        var deliverer = _delivererRepository.GetById(delivererId);

        if (deliverer == null)
            return false;
        
        if (deliverer.DriverLicense == DriversLicense.Car || deliverer.DriverLicense == DriversLicense.MotorcycleCar)
            return true;

        return false;
    }

    /// <summary>
    /// Creates a new deliverer.
    /// </summary>
    /// <param name="model">The deliverer DTO.</param>
    /// <param name="userId">The user ID.</param>
    /// <returns>The newly created deliverer or null if an error occurred.</returns>
    public Deliverer? CreateDeliverer(DelivererDto? model, Guid userId)
    {
        if (model == null)
            return null;
        
        var exist = _delivererRepository.GetByCnh(model.Cnh) ??
                      _delivererRepository.GetByPrimaryDocument(model.PrimaryDocument);

        if (exist != null)
            return null;

        var deliverer = Deliverer.Create();

        deliverer
            .SetName(model.Name)
            .SetCnh(model.Cnh)
            .SetPrimaryDocument(new CNPJ(model.PrimaryDocument))
            .SetDriverLicense(model.DriversLicense)
            .SetBirthday(model.Birthday)
            .SetUserId(userId);

        var success = _delivererRepository.Add(deliverer);

        if(!success)
            return null;

        return deliverer;
    }

    public bool UploadDocument(IFormFile file, Guid? delivererId)
    {
        try
        {
            if (delivererId == null)
                throw new ArgumentException("Deliverer not found.");
            
            var deliverer = _delivererRepository.GetById(delivererId.Value);

            if (deliverer == null)
                throw new ArgumentException("Deliverer not found.");
            
            const string directoryPath = @"C:\documentsImages";
            
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            
            var fullPath = Path.Combine(directoryPath, file.FileName);

            using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
            
            deliverer.SetDriverLicenseImage(fullPath);
            _delivererRepository.Update(deliverer);
            
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}