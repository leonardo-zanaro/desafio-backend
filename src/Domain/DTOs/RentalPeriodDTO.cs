namespace Domain.DTOs;

public class RentalPeriodBase
{
    public int Days { get; set; }
    public decimal DailyPrice { get; set; }
    public decimal PercentagePenalty { get; set; }
}
public class GetRentalPeriodDTO : RentalPeriodBase
{
    public Guid? Id { get; set; }
}

public class CreateRentalPeriodDTO : RentalPeriodBase
{
}
