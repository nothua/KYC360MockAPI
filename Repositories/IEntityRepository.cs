namespace KYC360MockAPI.Repositories;

using Models;

public interface IEntityRepository
{
    IEnumerable<Entity?> GetAllEntities();
    Entity? GetEntityById(string id);
    void AddEntity(Entity? entity);
    void UpdateEntity(Entity? entity);
    void DeleteEntity(string id);
}