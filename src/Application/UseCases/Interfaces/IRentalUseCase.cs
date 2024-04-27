using Application.ViewModel;

namespace Application.UseCases.Interfaces;

public interface IRentalUseCase
{
    Result RentMotorcycle(Guid delivererId, Guid motorcycleId, Guid rentalPeriodId);
    Result RentActive(Guid motorcycleId);
    Result ReturnMotorcycle(Guid motorcycleId);
}