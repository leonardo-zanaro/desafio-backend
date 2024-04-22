using Domain.Base;

namespace Domain.Entities;

public class RentalPeriod : BaseEntity
{
    public int Days { get; private set; }
    public decimal DailyPrice { get; private set; }
    public decimal PercentagePenalty { get; private set; }

    #region Setters
    public RentalPeriod SetDays(int days)
    {
        if (!int.IsPositive(days))
            throw new ArgumentException("It is necessary to have at least one day");
        
        Days = days;
        return this;
    }
    public RentalPeriod SetDailyPrice(decimal price)
    {
        if (price <= 0)
            throw new ArgumentException("The value cannot be negative or zero");
        
        DailyPrice = price;
        return this;
    }
    public RentalPeriod SetPercentagePenalty(decimal percentagePenalty)
    {
        PercentagePenalty = percentagePenalty;
        return this;
    }
    #endregion
}