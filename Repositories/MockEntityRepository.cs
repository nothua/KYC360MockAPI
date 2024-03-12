using System.Diagnostics;

namespace KYC360MockAPI.Repositories;

using Bogus;
using Models;
using System.Collections.Generic;

public class MockEntityRepository : IEntityRepository
{
    private readonly List<Entity?> _entities = [];
    private readonly Faker<Entity?> _entityFaker;

    public MockEntityRepository()
    {
        _entityFaker = new Faker<Entity?>()
            .RuleFor(e => e!.Id, f => f.Random.Guid().ToString())
            .RuleFor(e => e!.Deceased, f => f.Random.Bool())
            .RuleFor(e => e!.Gender, f => f.PickRandom<Gender>().ToString())
            .RuleFor(e => e!.Addresses, f => new List<Address>
            {
                new Address
                {
                    AddressLine = f.Address.StreetAddress(),
                    City = f.Address.City(),
                    Country = f.Address.Country()
                }
            })
            .RuleFor(e => e.Dates, f => new List<Date>
            {
                new Date
                {
                    DateType = "Birth",
                    DateValue = f.Date.Past(80)
                },
                
            })
            .RuleFor(e => e.Names, f => new List<Name>
            {
                new Name
                {
                    FirstName = f.Name.FirstName(),
                    MiddleName = f.Name.FirstName(),
                    Surname = f.Name.LastName()
                }
            });
    }

    public void GenerateMockData()
    {
        for (var i = 0; i < 10; i++)
        {
            _entities.Add(_entityFaker.Generate());
        }
    }
    
    public IEnumerable<Entity?> GetAllEntities()
    {
        return _entities;
    }

    Entity? IEntityRepository.GetEntityById(string id)
    {
        return GetEntityById(id);
    }

    void IEntityRepository.AddEntity(Entity? entity)
    {
        AddEntity(entity);
    }

    void IEntityRepository.UpdateEntity(Entity? entity)
    {
        UpdateEntity(entity);
    }

    IEnumerable<Entity?> IEntityRepository.GetAllEntities()
    {
        return GetAllEntities();
    }

    public Entity? GetEntityById(string id)
    {
        return _entities.Find(entity => entity?.Id == id);
    }

    public void AddEntity(Entity? entity)
    {
        _entities.Add(entity);
    }

    public void UpdateEntity(Entity? entity)
    {
        var index = _entities.FindIndex(e => e.Id == entity.Id);
        if (index != -1)
        {
            _entities[index] = entity;
        }
    }

    public void DeleteEntity(string id)
    {
        _entities.RemoveAll(entity => entity?.Id == id);
    }
}

public enum Gender
{
    Male,
    Female,
    Other
}