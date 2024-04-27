using Application.DTOs;
using Application.ViewModel;
using Microsoft.AspNetCore.Http;

namespace Application.UseCases.Interfaces;

public interface IDelivererUseCase
{
    IEnumerable<GetDelivererDTO> GetAll(int? page = null, int? pageQuantity = null);
    Result MotorcycleEnabled(Guid delivererId);
    Result CreateDeliverer(CreateDelivererDTO? model, Guid userId);
    Result UploadDocument(IFormFile file, Guid? delivererId);
}