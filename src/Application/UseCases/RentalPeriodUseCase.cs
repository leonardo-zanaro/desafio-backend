using Application.UseCases.Interfaces;
using Application.ViewModel;
using Domain.DTOs;
using Domain.Entities;
using Infra.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.UseCases;

public class RentalPeriodUseCase : IRentalPeriodUseCase
{
    private readonly IRentalPeriodRepository _rentalPeriodRepository;
    private readonly ILogger<RentalPeriodUseCase> _logger;
    public RentalPeriodUseCase(
        IRentalPeriodRepository   rentalPeriodRepository,
        ILogger<RentalPeriodUseCase> logger)
    {
        _rentalPeriodRepository = rentalPeriodRepository;
        _logger = logger;
    }
    
    public Result Create(RentalPeriodDTO model)
    {
        try
        {
            var period = RentalPeriod.Create();

            period
                .SetDays(model.Days)
                .SetDailyPrice(model.DailyPrice)
                .SetPercentagePenalty(model.PercentagePenalty);

            _rentalPeriodRepository.Add(period);

            var periodDto = new RentalPeriodDTO
            {
                Days = period.Days,
                DailyPrice = period.DailyPrice,
                PercentagePenalty = period.PercentagePenalty
            };
            
            return Result.ObjectResult(periodDto, "Rental period created successfully");
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex.Message);
            return Result.FailResult(ex.Message);
        }
    }

    public Result Remove(Guid rentalPeriodId)
    {
        try
        {
            var period = _rentalPeriodRepository.GetById(rentalPeriodId);
            
            if(period == null)
                return Result.FailResult("Rental Period not found.");
            
            _rentalPeriodRepository.Remove(rentalPeriodId);

            return Result.SuccessResult("Period was successfully removed");
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex.Message);
            return Result.FailResult(ex.Message);
        }
    }
}