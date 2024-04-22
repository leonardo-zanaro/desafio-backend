using Application.DTOs;
using Moq;
using Domain.Entities;
using Infra.Repositories.Interfaces;
using Application.UseCases.Interfaces;
using Infra.Context;
using Infra.Repositories;
using Microsoft.EntityFrameworkCore;

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
        _motorcycleRepository = new MotorcycleRepository(_context);
        _useCase = new MotorcycleUseCase(_motorcycleRepository);

        AddRandomMotorcycles();
    }
    
    [Test]
    public void CreateMotorcycle_ReturnsMotorcycle_WhenSuccessful()
    {
        var newMotorcycle = new MotorcycleDto()
        {
            Model = "Honda CB750",
            Year = "1969",
            LicensePlate = "ABC2345"
        };

        var createdMotorcycle = _useCase.CreateMotorcycle(newMotorcycle);

        var motorcycle = _useCase.GetById(createdMotorcycle.Id);
        
        Assert.AreEqual(newMotorcycle.Model, motorcycle.Model);
        Assert.AreEqual(newMotorcycle.Year, motorcycle.Year);
        Assert.AreEqual(newMotorcycle.LicensePlate, motorcycle.LicensePlate);
    }
    
    [Test]
    public void CreateMotorcycle_ReturnsNull_WhenError()
    {
        var newMotorcycle = new MotorcycleDto()
        {
            Year = "1969",
            LicensePlate = "ABC2345"
        };

        var createdMotorcycle = _useCase.CreateMotorcycle(newMotorcycle);

        Assert.Null(createdMotorcycle);
    }
    //
    // [Test]
    // public void ChangeLicensePlate_ReturnsTrue_WhenMotorcycleExisting()
    // {
    //     var plate = "ABC123";
    //
    //     var motorcycle = Motorcycle.Create();
    //
    //     motorcycle
    //         .SetLicensePlate("123456")
    //         .SetModel("Honda CB750")
    //         .SetYear("1969");
    //     
    //     _motorcycleRepository.Setup(r => r.GetById(motorcycle.Id))
    //         .Returns(motorcycle);
    //     _motorcycleRepository.Setup(r => r.Update(motorcycle))
    //         .Returns(true);
    //
    //     var result = _useCase.ChangePlate(motorcycle.Id, plate);
    //
    //     Assert.True(result);
    // }
    //
    // [Test]
    // public void ChangeLicensePlate_ReturnsFalse_WhenMotorcycleNotExisting()
    // {
    //     var id = Guid.NewGuid();
    //     var plate = "ABC123";
    //     
    //     _motorcycleRepository.Setup(r => r.GetById(id))
    //         .Returns((Motorcycle?)null);
    //
    //     var result = _useCase.ChangePlate(id, plate);
    //
    //     Assert.False(result);
    // }
    //
    // [Test]
    // public void GetByPlate_ReturnsMotorcycle_WhenMotorcycleExisting()
    // {
    //     var plate = "ABC1234";
    //
    //     var motorcycle = Motorcycle.Create();
    //
    //     motorcycle
    //         .SetLicensePlate(plate)
    //         .SetModel("Honda CB750")
    //         .SetYear("1969");
    //
    //     _motorcycleRepository.Setup(r => r.GetByPlate(plate)).Returns(motorcycle);
    //
    //     var result = _useCase.GetByPlate(plate);
    //
    //     Assert.NotNull(result);
    //     Assert.That(result!.LicensePlate, Is.EqualTo(plate));
    // }
    //
    // [Test]
    // public void GetByPlate_ReturnsNull_WhenPlateNotFound()
    // {
    //     var plate = "ABC1234";
    //
    //     var motorcycle = Motorcycle.Create();
    //
    //     motorcycle
    //         .SetLicensePlate(plate)
    //         .SetModel("Honda CB750")
    //         .SetYear("1969");
    //
    //     _motorcycleRepository.Setup(r => r.GetByPlate(plate)).Returns(motorcycle);
    //
    //     var result = _useCase.GetByPlate("XPT1234");
    //
    //     Assert.Null(result);
    //     Assert.That(result!.LicensePlate, Is.Not.EqualTo(plate));
    // }
    //
    // [Test]
    // public void RemoveMotorcycle_ReturnsTrue_WhenMotorcycleExisting()
    // {
    //     var motorcycle = Motorcycle.Create();
    //
    //     motorcycle
    //         .SetLicensePlate("123456")
    //         .SetModel("Honda CB750")
    //         .SetYear("1969");
    //     
    //     _motorcycleRepository.Setup(r => r.GetById(motorcycle.Id))
    //         .Returns(motorcycle);
    //     
    //     var result = _useCase.RemoveMotorcycle(motorcycle.Id);
    //
    //     Assert.True(result);
    // }
    
    private void AddRandomMotorcycles(int count = 100)
    {
        #region Add Default Motorcycle
        var motorcyle = Motorcycle.Create();

        motorcyle
            .SetLicensePlate("ABC1234")
            .SetModel("Honda CB750")
            .SetYear("1969");

        _context.Add(motorcyle);
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

            motorcycle
                .SetLicensePlate("XYZ" + rand.Next(1000, 9999))
                .SetModel(models[rand.Next(models.Length)])
                .SetYear((rand.Next(1960, 2023)).ToString());

            _context.Add(motorcycle);
        }

        _context.SaveChanges();
    }
}