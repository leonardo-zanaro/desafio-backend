namespace Domain.DTOs;

public class RentalPeriodDTO
{
    public int Days { get; set; }
    public decimal DailyPrice { get; set; }
    public decimal PercentagePenalty { get; set; }
}