using Application.UseCases.Interfaces;
using Application.ViewModel;
using Domain.Entities;
using Domain.Enums;
using Infra.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.UseCases;

public class RentalUseCase : IRentalUseCase
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IRentalPeriodRepository _rentalPeriodRepository;
    private readonly IMotorcycleRepository _motorcycleRepository;
    private readonly ILogger<RentalUseCase> _logger;

    public RentalUseCase(
        IRentalRepository rentalRepository,
        IRentalPeriodRepository rentalPeriodRepository,
        IMotorcycleRepository motorcycleRepository,
        ILogger<RentalUseCase> logger)
    {
        _rentalRepository = rentalRepository;
        _rentalPeriodRepository = rentalPeriodRepository;
        _motorcycleRepository = motorcycleRepository;
        _logger = logger;
    }

    public Result RentMotorcycle(Guid delivererId, Guid motorcycleId, Guid rentalPeriodId)
    {
        try
        {
            var oldRental = _rentalRepository.RentalByMotorcycleId(motorcycleId);

            if (oldRental != null && oldRental.StartDate >= DateTime.Today)
                return Result.FailResult("Motorcycle is already rented.");
            
            var period = _rentalPeriodRepository.GetById(rentalPeriodId);

            if (period == null)
                return Result.FailResult("Period not found.");
            
            var periodDays = period.Days + 1;
            var endDate = DateTime.UtcNow.AddDays(periodDays);
            
            var rental = Rental.Create();

            rental
                .SetDelivererId(delivererId)
                .SetMotorcycleId(motorcycleId)
                .SetRentalPeriodId(rentalPeriodId)
                .SetExpectedCompletionDate(endDate);

            _rentalRepository.Add(rental);

            return Result.SuccessResult();
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex.Message);
            return Result.SuccessResult();
        }
    }

    public Result RentActive(Guid motorcycleId)
    {
        try
        {
            var rental = _rentalRepository.RentalByMotorcycleId(motorcycleId);

            if (rental == null)
                return Result.SuccessResult();
                

            return Result.FailResult("Motorcycle in use.");
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex.Message);
            return Result.FailResult(ex.Message);
        }
    }


    public Result ReturnMotorcycle(Guid motorcycleId)
    {
        try
        {
            var motorcycle = _motorcycleRepository.GetById(motorcycleId);

            if (motorcycle == null)
                return Result.FailResult("Motorcycle not found.");
            
            var rental = _rentalRepository.RentalByMotorcycleId(motorcycleId);

            if (rental == null)
                return Result.FailResult("Rental not found.");
            
            var rentalPeriod = _rentalPeriodRepository.GetById(rental.RentalPeriodId);

            if (rentalPeriod == null)
                return Result.FailResult("Period not found.");
            
            
            var fine = CalculateFine(rentalPeriod, rental);

            rental
                .SetEndDate()
                .SetFine(fine);

            _rentalRepository.Update(rental);
            
            motorcycle
                .SetStatus(StatusMotorcycle.Avaliable);

            _motorcycleRepository.Update(motorcycle);

            return Result.SuccessResult();
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex.Message);
            return Result.FailResult(ex.Message);
        }
    }
    
    private decimal CalculateFine(RentalPeriod period, Rental rental)
    {
        var days = period.Days;
        var calculateDays = (DateTime.Today - rental.StartDate).Days;
        var daysRemaining = days - calculateDays;

        decimal result;
        
        if (daysRemaining > 0)
        {
            var calculateDaily = period.DailyPrice * daysRemaining;
            var penalty = calculateDaily * (period.PercentagePenalty / 100);

            result = calculateDaily + penalty;   
        }
        else
        {
            var extraCharge = Math.Abs(daysRemaining) * 50;
            result = extraCharge;
        }

        return result;
    }
}