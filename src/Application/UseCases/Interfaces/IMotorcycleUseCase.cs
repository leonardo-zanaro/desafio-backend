using Application.DTOs;
using Domain.Entities;

namespace Application.UseCases.Interfaces;

public interface IMotorcycleUseCase
{
    Motorcycle CreateMotorcycle(MotorcycleDto model);
    IEnumerable<Motorcycle> GetAll();
    Motorcycle? GetById(Guid motorcycleId);
    Motorcycle? BringAvailable(Guid motorcycleId);
    Motorcycle? GetByPlate(string plate);
    bool ChangePlate(Guid motorcycleId, string plate);
    bool RemoveMotorcycle(Guid motorcycleId);
}