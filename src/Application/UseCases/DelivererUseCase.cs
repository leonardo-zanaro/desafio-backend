using Application.DTOs;
using Application.UseCases.Interfaces;
using Application.ViewModel;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using Infra.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Application.UseCases;

public class DelivererUseCase : IDelivererUseCase
{
    private readonly IDelivererRepository _delivererRepository;
    private readonly ILogger<Deliverer> _logger;

    public DelivererUseCase(IDelivererRepository delivererRepository, ILogger<Deliverer> logger)
    {
        _delivererRepository = delivererRepository;
        _logger = logger;
    }

    public Result MotorcycleEnabled(Guid delivererId)
    {
        try
        {
            var deliverer = _delivererRepository.GetById(delivererId);

            if (deliverer == null)
                return Result.FailResult("Deliverer not found.");

            if (deliverer.DriverLicense == DriversLicense.Motorcycle ||
                deliverer.DriverLicense == DriversLicense.MotorcycleCar)
                return Result.SuccessResult();

            return Result.FailResult("Unlicensed driver");
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex.Message);
            return Result.FailResult(ex.Message);
        }
    }

    public Result CreateDeliverer(DelivererDTO? model, Guid userId)
    {
        try
        {
            if (model == null)
                return Result.FailResult("Model invalid.");
            
            var existCnh = _delivererRepository.GetByCnh(model.Cnh);

            if (existCnh != null)
                return Result.FailResult("Existing driver's license");

            var existCnpj = _delivererRepository.GetByPrimaryDocument(model.PrimaryDocument);
            
            if (existCnpj != null)
                return Result.FailResult("Existing cnpj");

            var deliverer = Deliverer.Create();

            deliverer
                .SetName(model.Name)
                .SetCnh(model.Cnh)
                .SetPrimaryDocument(new CNPJ(model.PrimaryDocument))
                .SetDriverLicense(model.DriversLicense)
                .SetBirthday(model.Birthday)
                .SetUserId(userId);

            _delivererRepository.Add(deliverer);

            var delivererDto = new DelivererDTO
            {
                PrimaryDocument = deliverer.PrimaryDocument,
                Birthday = deliverer.Birthday,
                Cnh = deliverer.Cnh,
                Name = deliverer.Name,
                DriversLicense = deliverer.DriverLicense
            };
            
            return Result.ObjectResult(delivererDto);
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex.Message);
            return Result.FailResult(ex.Message);
        }
    }

    public Result UploadDocument(IFormFile file, Guid? delivererId)
    {
        try
        {
            if (delivererId == null)
                return Result.FailResult("DelivererId invalid.");
            
            var deliverer = _delivererRepository.GetById(delivererId.Value);

            if (deliverer == null)
                return Result.FailResult("Deliverer not found.");
            
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
            
            return Result.SuccessResult();
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex.Message);
            return Result.FailResult(ex.Message);
        }
    }
}