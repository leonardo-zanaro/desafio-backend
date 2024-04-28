using Application.ViewModel;
using Domain.DTOs;

namespace Application.UseCases.Interfaces;

public interface IRentalPeriodUseCase
{
    Result Create(CreateRentalPeriodDTO model);
    Result Remove(Guid rentalPeriodId);
    IEnumerable<GetRentalPeriodDTO> GetAll(int? page = null, int? pageQuantity = null);
}