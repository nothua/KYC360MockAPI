namespace KYC360MockAPI.Repositories;

using Models;


public interface IEntityRepository
{
    Task<bool> AddEntityAsync(Entity entity);
    Task<bool> UpdateEntityAsync(Entity entity);
    Task<bool> DeleteEntityAsync(string id);
    Task<Entity?> GetEntityByIdAsync(string id);
    Task<IEnumerable<Entity>> GetAllEntitiesAsync();
}