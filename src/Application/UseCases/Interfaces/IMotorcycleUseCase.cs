using Application.DTOs;
using Application.ViewModel;

namespace Application.UseCases.Interfaces;

public interface IMotorcycleUseCase
{
    Result CreateMotorcycle(CreateMotorcycleDTO model);
    IEnumerable<GetMotorcycleDTO> GetAll(int? page = null, int? pageQuantity = null);
    Result GetById(Guid motorcycleId);
    Result BringAvailable(Guid motorcycleId);
    Result GetByPlate(string plate);
    Result ChangePlate(Guid motorcycleId, string plate);
    Result RemoveMotorcycle(Guid motorcycleId);
}