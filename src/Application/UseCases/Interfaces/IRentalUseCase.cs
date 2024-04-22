namespace Application.UseCases.Interfaces;

public interface IRentalUseCase
{
    bool RentMotorcycle(Guid delivererId, Guid motorcycleId, Guid rentalPeriodId);
    bool RentActive(Guid motorcycleId);
    bool ReturnMotorcycle(Guid motorcycleId);
}