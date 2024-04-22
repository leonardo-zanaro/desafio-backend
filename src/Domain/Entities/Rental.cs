using Domain.Base;

namespace Domain.Entities;

public class Rental : BaseEntity
{
    public Guid DelivererId { get; private set; }
    public Guid MotorcycleId { get; private set; }
    public Guid RentalPeriodId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set;}
    public DateTime? ExpectedCompletionDate { get; private set; }
    public decimal? Fine { get; private set; }

    #region Setters
    public Rental SetDelivererId(Guid delivererId)
    {
        DelivererId = delivererId;
        return this;
    }
    public Rental SetMotorcycleId(Guid motorcycleId)
    {
        MotorcycleId = motorcycleId;
        return this;
    }
    public Rental SetRentalPeriodId(Guid rentalPeriodId)
    {
        RentalPeriodId = rentalPeriodId;
        return this;
    }
    public Rental SetStartDate()
    {
        StartDate = DateTime.UtcNow;
        return this;
    }
    public Rental SetEndDate()
    {
        EndDate = DateTime.UtcNow;
        return this;
    }
    public Rental SetExpectedCompletionDate(DateTime? endDate)
    {
        ExpectedCompletionDate = endDate;
        return this;
    }
    public Rental SetFine(decimal fine)
    {
        Fine = fine;
        return this;
    }
    #endregion

    public static Rental Create()
    {
        var rental =  new Rental();

        rental.StartDate = DateTime.UtcNow.AddDays(1);
        
        return rental;
    }
}