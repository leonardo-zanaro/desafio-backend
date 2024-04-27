using Application.DTOs;
using Application.ViewModel;
using Domain.Entities;

namespace Application.UseCases.Interfaces;

public interface IMotorcycleUseCase
{
    Result CreateMotorcycle(MotorcycleDTO model);
    IEnumerable<MotorcycleDTO> GetAll(int? page = null, int? pageQuantity = null);
    Result GetById(Guid motorcycleId);
    Result BringAvailable(Guid motorcycleId);
    Result GetByPlate(string plate);
    Result ChangePlate(Guid motorcycleId, string plate);
    Result RemoveMotorcycle(Guid motorcycleId);
}