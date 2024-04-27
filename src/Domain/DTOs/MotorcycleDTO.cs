namespace Application.DTOs 
{
    public class MotorcycleBase
    {
        public string LicensePlate { get; set; }
        public string Year { get; set; }
        public string Model { get; set; }
    }
    
    public class GetMotorcycleDTO : MotorcycleBase
    {
        public Guid? Id { get; set; }
    }

    public class CreateMotorcycleDTO : MotorcycleBase
    {
    }
}