using Application.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.UseCases.Interfaces;

public interface IDelivererUseCase
{
    bool MotorcycleEnabled(Guid delivererId);
    bool CarEnabled(Guid delivererId);
    Deliverer? CreateDeliverer(DelivererDto? model, Guid userId);
    bool UploadDocument(IFormFile file, Guid? delivererId);
}