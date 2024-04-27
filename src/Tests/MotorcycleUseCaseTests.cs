using Application.DTOs;
using Domain.Entities;
using Infra.Repositories.Interfaces;
using Application.UseCases.Interfaces;
using Infra.Context;
using Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ILogger = Castle.Core.Logging.ILogger;

namespace DesafioBackEnd.Tests;

public class MotorcycleUseCaseTests
{
    private MotorcycleUseCase _useCase;
    private IMotorcycleRepository _motorcycleRepository;
    private DmContext _context;
    [SetUp]
    public void SetUp()
    {
        
        var options = new DbContextOptionsBuilder<DmContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;
        
        _context = new DmContext(options);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
        ILogger<MotorcycleRepository> loggerRepository = new LoggerFactory().CreateLogger<MotorcycleRepository>();
        _motorcycleRepository = new MotorcycleRepository(_context, loggerRepository);
        ILogger<MotorcycleUseCase> logger = new LoggerFactory().CreateLogger<MotorcycleUseCase>();
        _useCase = new MotorcycleUseCase(_motorcycleRepository, logger);

        AddRandomMotorcycles();
    }
    
    [Test]
    public void CreateMotorcycle_ReturnsMotorcycle_WhenSuccessful()
    {
        var newMotorcycle = new MotorcycleDTO()
        {
            Model = "Honda CB750",
            Year = "1969",
            LicensePlate = "ABC2345"
        };

        var createdMotorcycle = _useCase.CreateMotorcycle(newMotorcycle).Object as MotorcycleDTO;

        Assert.NotNull(createdMotorcycle);
        var motorcycle = _useCase.GetById(createdMotorcycle.Id.Value).Object as MotorcycleDTO;
    
        Assert.NotNull(motorcycle);
        Assert.That(motorcycle.Model, Is.EqualTo(newMotorcycle.Model));
        Assert.That(motorcycle.Year, Is.EqualTo(newMotorcycle.Year));
        Assert.That(motorcycle.LicensePlate, Is.EqualTo(newMotorcycle.LicensePlate));
    }
    
    [Test]
    public void CreateMotorcycle_ReturnsNull_WhenError()
    {
        var newMotorcycle = new MotorcycleDTO()
        {
            Model = "Honda CB750",
            Year = "1969"
        };

        var createdMotorcycle = _useCase.CreateMotorcycle(newMotorcycle).Object;

        Assert.Null(createdMotorcycle);
    }

    [Test]
    public void GetAllMotorcycle_ReturnsListMotorcycle_WhenExcludedIsFalse()
    {
        var motorcycles =_useCase.GetAll();
        
        Assert.IsInstanceOf<IEnumerable<MotorcycleDTO>>(motorcycles);
    }
    
    [Test]
    public void GetMotorcycleByLicensePlate_ReturnsMotorcycle_WhenMotorcycleFound()
    {
        var licensePlate = "ABC1234";

        var motorcycle = _useCase.GetByPlate(licensePlate).Object as Motorcycle;
        
        Assert.IsNotNull(motorcycle);
        Assert.That(licensePlate, Is.EqualTo(motorcycle.LicensePlate));
        Assert.IsInstanceOf<Motorcycle>(motorcycle);
    }
    
    [Test]
    public void GetMotorcycleByLicensePlate_ReturnsNull_WhenMotorcycleNotFound()
    {
        var licensePlate = "XXXYYYZZZ";

        var motorcycle = _useCase.GetByPlate(licensePlate).Object;
        
        Assert.IsNull(motorcycle);
    }
    
    [Test]
    public void UpdateLicensePlate_ReturnsFalse_WhenLicensePlateInUse()
    {
        var newLicensePlate = "XYZ1234";
        
        var motorcycle = _useCase.GetAll().First();

        var result =_useCase.ChangePlate(motorcycle.Id.Value, newLicensePlate);
        
        Assert.IsFalse(result.Success);
    }
    
    [Test]
    public void UpdateLicensePlate_ReturnsTrue_WhenSuccess()
    {
        var newLicensePlate = "BZX9999";
        
        var motorcycle = _useCase.GetAll().First();

        var result =_useCase.ChangePlate(motorcycle.Id.Value, newLicensePlate);
        
        Assert.IsTrue(result.Success);
    }
     
    private void AddRandomMotorcycles(int count = 100)
    {
        #region Add Default Motorcycle
        var motorcyle1 = Motorcycle.Create();

        motorcyle1
            .SetLicensePlate("ABC1234")
            .SetModel("Honda CB750")
            .SetYear("1969");

        _context.Add(motorcyle1);
        
        var motorcyle2 = Motorcycle.Create();

        motorcyle2
            .SetLicensePlate("XYZ1234")
            .SetModel("Honda CB750")
            .SetYear("1970");

        _context.Add(motorcyle2);
        #endregion
        
        var models = new [] 
        { 
            "Honda CB750", 
            "Yamaha MT-07", 
            "Ducati Panigale V4", 
            "BMW R1250GS", 
            "Harley Davidson Iron 883",
            "KTM 390 Duke",
            "Kawasaki Ninja ZX-10R",
            "Triumph Street Triple",
            "Suzuki Hayabusa",
            "Royal Enfield Classic 350"
        };

        var rand = new Random();

        for (int i = 0; i < count; i++)
        {
            var motorcycle = Motorcycle.Create();
            
            motorcycle.Excluded = (i % 3 == 0);
            
            motorcycle
                .SetLicensePlate("XYZ" + rand.Next(1000, 9999))
                .SetModel(models[rand.Next(models.Length)])
                .SetYear((rand.Next(1960, 2023)).ToString());

            _context.Add(motorcycle);
        }

        _context.SaveChanges();
    }
}