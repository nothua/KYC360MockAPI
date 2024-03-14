using KYC360MockAPI.Controllers;
using KYC360MockAPI.Models;
using KYC360MockAPI.Repositories;
using KYC360MockAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace KYC360MockAPI.Tests;

[TestFixture]
public class EntityControllerTests
{
    private List<Entity> entities;
    private EntityController mockController;
    private Mock<IEntityRepository> mockEntityRepository;
    private Mock<ILogger<EntityController>> mockLogger;

    [SetUp]
    public void Setup()
    {
        mockEntityRepository = new Mock<IEntityRepository>();
        mockLogger = new Mock<ILogger<EntityController>>();
        
        entities = MockDataGenerator.GenerateMockEntities(10);
        mockEntityRepository.Setup(repo => repo.GetAllEntitiesAsync()).ReturnsAsync(entities);

        mockController = new EntityController(mockEntityRepository.Object, mockLogger.Object);
    }
    
    [Test]
    public async Task GetEntities_Success()
    {
        var result = await mockController.GetEntities();

        var okResult = result as OkObjectResult;
        ClassicAssert.IsNotNull(okResult);
        var model = okResult.Value as dynamic;
        ClassicAssert.IsNotNull(model);
        ClassicAssert.AreEqual(entities.Count, model.Entities.Count, "Check entities count is same");
    }
    
    [Test]
    public async Task GetEntity_Success()
    {
        var entityId = "1234567890"; 
        var entity = new Entity(); 
        mockEntityRepository.Setup(repo => repo.GetEntityByIdAsync(entityId)).ReturnsAsync(entity);

        var actionResult = await mockController.GetEntity(entityId);
        
        ClassicAssert.IsNotNull(actionResult);
        ClassicAssert.AreEqual(entity, actionResult.Value, "Check if entity is the same as defined");
    }
    
    [Test]
    public async Task CreateEntity_Success()
    {
        var entity = new Entity
        {
            Id = "1234567890", 
            Deceased = false,
            Gender = "Male",
            Names = new List<Name>
            {
                new Name
                {
                    FirstName = "John",
                    MiddleName = "Robert",
                    Surname = "Doe"
                }
            },
            Addresses = new List<Address>
            {
                new Address
                {
                    AddressLine = "123 Main Street",
                    City = "Anytown",
                    Country = "United States"
                }
            },
            Dates = new List<Date>
            {
                new Date
                {
                    DateType = "Birth",
                    DateValue = new DateTime(1990, 5, 15) 
                }
            }
        };
        
        mockEntityRepository.Setup(repo => repo.GetEntityByIdAsync(entity.Id)).ReturnsAsync((Entity)null);
        mockEntityRepository.Setup(repo => repo.AddEntityAsync(entity)).ReturnsAsync(true);

        var result = await mockController.CreateEntity(entity);

        var okResult = result as OkObjectResult;
        ClassicAssert.IsNotNull(okResult);
        ClassicAssert.AreEqual("Created Successfully!", okResult.Value);
    }
    
    [Test]
    public async Task UpdateEntity_Success()
    {
        var entityId = "1234567890"; 
        var entity = new Entity
        {
            Id = "1234567890", 
            Deceased = false,
            Gender = "Male",
            Names = new List<Name>
            {
                new Name
                {
                    FirstName = "John",
                    MiddleName = "Robert",
                    Surname = "Doe"
                }
            },
            Addresses = new List<Address>
            {
                new Address
                {
                    AddressLine = "123 Main Street",
                    City = "Anytown",
                    Country = "United States"
                }
            },
            Dates = new List<Date>
            {
                new Date
                {
                    DateType = "Birth",
                    DateValue = new DateTime(1990, 5, 15) // Provide a valid date
                }
            }
        };

        mockEntityRepository.Setup(repo => repo.GetEntityByIdAsync(entityId)).ReturnsAsync(new Entity());
        mockEntityRepository.Setup(repo => repo.UpdateEntityAsync(entity)).ReturnsAsync(true);

        var result = await mockController.UpdateEntity(entityId, entity);

        var okResult = result as OkObjectResult;
        ClassicAssert.IsNotNull(okResult);
        ClassicAssert.AreEqual("Updated Successfully!", okResult.Value);
    }
    
    [Test]
    public async Task DeleteEntity_Success()
    {
        var entityId = "1234567890"; 
        mockEntityRepository.Setup(repo => repo.GetEntityByIdAsync(entityId)).ReturnsAsync(new Entity());
        mockEntityRepository.Setup(repo => repo.DeleteEntityAsync(entityId)).ReturnsAsync(true);

        var result = await mockController.DeleteEntity(entityId);

        var okResult = result as OkObjectResult;
        ClassicAssert.IsNotNull(okResult);
        ClassicAssert.AreEqual("Deleted Successfully!", okResult.Value);
    }
    
    
    
    [Test]
    public async Task FilterEntities_FilterByGender_ReturnsFilteredEntities()
    {
        var result = await mockController.FilterEntities(gender: "Male", startDate: null, endDate: null, countries: null, 1, 10);

        var okResult = result as OkObjectResult;
        ClassicAssert.IsNotNull(okResult);
        var model = okResult.Value as dynamic;
        var filteredEntities = model.Entities as List<Entity>;
        ClassicAssert.IsNotNull(filteredEntities);
        ClassicAssert.IsTrue(filteredEntities.All(e => e.Gender == "Male"), "All returned entities should be male");
    }
    
    [Test]
    public async Task SearchEntities_Success()
    {
        var searchQuery = "John"; 
        var entities = new List<Entity>
        {
            new Entity
            {
                Id = "1",
                Names = new List<Name>
                {
                    new Name
                    {
                        FirstName = "John",
                        MiddleName = "Robert",
                        Surname = "Doe"
                    }
                },
                Addresses = new List<Address>
                {
                    new Address
                    {
                        AddressLine = "123 Main Street",
                        City = "Anytown",
                        Country = "United States"
                    }
                }
            },
            new Entity
            {
                Id = "2",
                Names = new List<Name>
                {
                    new Name
                    {
                        FirstName = "Jane",
                        MiddleName = "Alice",
                        Surname = "Smith"
                    }
                },
                Addresses = new List<Address>
                {
                    new Address
                    {
                        AddressLine = "456 Oak Avenue",
                        City = "Somewhere",
                        Country = "United States"
                    }
                }
            }
        };

        mockEntityRepository.Setup(repo => repo.GetAllEntitiesAsync()).ReturnsAsync(entities);

        var result = await mockController.SearchEntities(searchQuery, 1, 10);
        
        var okResult = result as OkObjectResult;
        ClassicAssert.IsNotNull(okResult);
        var model = okResult.Value as dynamic;
        ClassicAssert.IsNotNull(model);
        ClassicAssert.AreEqual(1, model.Entities.Count, "Only one entity should match the search query");
        ClassicAssert.AreEqual(entities[0], model.Entities[0], "Returned entity should match the expected entity");
    }
    
    
}