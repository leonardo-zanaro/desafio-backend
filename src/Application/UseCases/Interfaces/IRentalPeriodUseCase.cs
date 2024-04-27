using Application.ViewModel;
using Domain.DTOs;

namespace Application.UseCases.Interfaces;

public interface IRentalPeriodUseCase
{
    Result Create(RentalPeriodDTO model);
    Result Remove(Guid rentalPeriodId);
    IEnumerable<RentalPeriodDTO> GetAll(int? page = null, int? pageQuantity = null);
}