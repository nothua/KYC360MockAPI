namespace KYC360MockAPI.Services;

using Bogus;
using Models;

public class MockDataGenerator
{
    public static List<Entity> GenerateMockEntities(int count)
    {
        var faker = new Faker<Entity>()
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
            .RuleFor(e => e.Dates, f =>
            [
                new Date
                {
                    DateType = "Birth",
                    DateValue = f.Date.Past(80)
                }


            ])
            .RuleFor(e => e.Names, f =>
            [
                new Name
                {
                    FirstName = f.Name.FirstName(),
                    MiddleName = f.Name.FirstName(),
                    Surname = f.Name.LastName()
                }
            ]);

        return faker.Generate(count);
    }
}
