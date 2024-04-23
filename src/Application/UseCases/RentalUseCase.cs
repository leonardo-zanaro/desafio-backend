using Application.UseCases.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infra.Repositories.Interfaces;

namespace Application.UseCases;

public class RentalUseCase : IRentalUseCase
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IRentalPeriodRepository _rentalPeriodRepository;
    private readonly IMotorcycleRepository _motorcycleRepository;

    public RentalUseCase(IRentalRepository rentalRepository, IRentalPeriodRepository rentalPeriodRepository, IMotorcycleRepository motorcycleRepository)
    {
        _rentalRepository = rentalRepository;
        _rentalPeriodRepository = rentalPeriodRepository;
        _motorcycleRepository = motorcycleRepository;
    }

    /// <summary>
    /// Rent a motorcycle for a specific rental period.
    /// </summary>
    /// <param name="delivererId">The ID of the deliverer.</param>
    /// <param name="motorcycleId">The ID of the motorcycle.</param>
    /// <param name="rentalPeriodId">The ID of the rental period.</param>
    /// <returns>True if the motorcycle was successfully rented, false otherwise.</returns>
    public bool RentMotorcycle(Guid delivererId, Guid motorcycleId, Guid rentalPeriodId)
    {
        try
        {
            var oldRental = _rentalRepository.RentalByMotorcycleId(motorcycleId);

            if (oldRental != null && oldRental.StartDate >= DateTime.Today)
                return false;
            
            var period = _rentalPeriodRepository.GetById(rentalPeriodId);

            if (period == null)
                throw new Exception("Period not found.");
            
            var periodDays = period.Days + 1;
            var endDate = DateTime.UtcNow.AddDays(periodDays);
            
            var rental = Rental.Create();

            rental
                .SetDelivererId(delivererId)
                .SetMotorcycleId(motorcycleId)
                .SetRentalPeriodId(rentalPeriodId)
                .SetExpectedCompletionDate(endDate);

            _rentalRepository.Add(rental);
            
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Checks if a motorcycle is currently rented.
    /// </summary>
    /// <param name="motorcycleId">The ID of the motorcycle to check.</param>
    /// <returns>Returns true if the motorcycle is currently rented, otherwise false.</returns>
    public bool RentActive(Guid motorcycleId)
    {
        try
        {
            var rental = _rentalRepository.RentalByMotorcycleId(motorcycleId);

            if (rental == null)
                throw new Exception("Rental not found.");
            
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Returns a motorcycle and ends the rental period.
    /// </summary>
    /// <param name="motorcycleId">The ID of the motorcycle to be returned.</param>
    /// <returns>Returns true if the motorcycle was successfully returned and false otherwise.</returns>
    public bool ReturnMotorcycle(Guid motorcycleId)
    {
        try
        {
            var motorcycle = _motorcycleRepository.GetById(motorcycleId);
            
            if(motorcycle == null)
                throw new Exception("Motorcycle not found.");
            
            var rental = _rentalRepository.RentalByMotorcycleId(motorcycleId);
            
            if (rental == null)
               throw new Exception("Rental not found.");
            
            var rentalPeriod = _rentalPeriodRepository.GetById(rental.RentalPeriodId);
            
            if (rentalPeriod == null)
                return false;
            
            var fine = CalculateFine(rentalPeriod, rental);

            rental
                .SetEndDate()
                .SetFine(fine);

            _rentalRepository.Update(rental);
            
            motorcycle
                .SetStatus(StatusMotorcycle.Avaliable);

            _motorcycleRepository.Update(motorcycle);
            
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Calculates the fine for a rental period based on the remaining days and penalty percentage.
    /// </summary>
    /// <param name="period">The rental period.</param>
    /// <param name="rental">The rental information.</param>
    /// <returns>The calculated fine amount.</returns>
    private decimal CalculateFine(RentalPeriod period, Rental rental)
    {
        var days = period.Days;
        var calculateDays = (DateTime.Today - rental.StartDate).Days;
        var daysRemaining = days - calculateDays;

        decimal result = 0;
        
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