using Application.DTOs;
using Application.ViewModel;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.UseCases.Interfaces;

public interface IDelivererUseCase
{
    Result MotorcycleEnabled(Guid delivererId);
    Result CreateDeliverer(DelivererDTO? model, Guid userId);
    Result UploadDocument(IFormFile file, Guid? delivererId);
}