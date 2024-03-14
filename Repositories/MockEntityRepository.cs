namespace KYC360MockAPI.Repositories;

using Models;
using System.Collections.Generic;

public class MockEntityRepository(IDatabase database) : IEntityRepository
{
    private readonly IDatabase _database = database ?? throw new ArgumentNullException(nameof(database));

    public async Task<bool> AddEntityAsync(Entity entity)
    {
        return await _database.AddEntityAsync(entity);
    }

    public async Task<bool> UpdateEntityAsync(Entity entity)
    {
        return await _database.UpdateEntityAsync(entity);
    }

    public async Task<bool> DeleteEntityAsync(string id)
    {
        return await _database.DeleteEntityAsync(id);
    }

    public async Task<Entity?> GetEntityByIdAsync(string id)
    {
        return await _database.GetEntityByIdAsync(id);
    }

    public async Task<IEnumerable<Entity>> GetAllEntitiesAsync()
    {
        return await _database.GetAllEntitiesAsync();
    }
}
