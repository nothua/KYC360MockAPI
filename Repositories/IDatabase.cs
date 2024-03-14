namespace KYC360MockAPI.Repositories;

using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IDatabase
{
    Task<bool> AddEntityAsync(Entity entity);
    Task<bool> UpdateEntityAsync(Entity entity);
    Task<bool> DeleteEntityAsync(string id);
    Task<Entity?> GetEntityByIdAsync(string id);
    Task<IEnumerable<Entity>> GetAllEntitiesAsync();
}