namespace KYC360MockAPI.Repositories;

using Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class MockDatabase(ILogger<MockDatabase> logger) : IDatabase
{
    private readonly List<Entity> _entities = [];

    public async Task<bool> AddEntityAsync(Entity entity)
    {
        return await RetryWithBackoffAsync(() =>
        {
            _entities.Add(entity);
            logger.LogInformation($"Entity added: {entity.Id}");
            return Task.FromResult(true);
        });
    }

    public async Task<bool> UpdateEntityAsync(Entity entity)
    {
        return await RetryWithBackoffAsync(() =>
        {
            var existingEntity = _entities.Find(e => e.Id == entity.Id);
            if (existingEntity == null)
            {
                logger.LogError($"Entity not found for update: {entity.Id}");
                return Task.FromResult(false);
            }

            existingEntity.Deceased = entity.Deceased;
            existingEntity.Gender = entity.Gender;
            existingEntity.Names = entity.Names;
            existingEntity.Addresses = entity.Addresses;
            existingEntity.Dates = entity.Dates;

            logger.LogInformation($"Entity updated: {entity.Id}");
            return Task.FromResult(true);
        });
    }

    public async Task<bool> DeleteEntityAsync(string id)
    {
        return await RetryWithBackoffAsync(() =>
        {
            var entityToRemove = _entities.Find(e => e.Id == id);
            if (entityToRemove == null)
            {
                logger.LogError($"Entity not found for delete: {id}");
                return Task.FromResult(false);
            }

            _entities.Remove(entityToRemove);
            logger.LogInformation($"Entity deleted: {id}");
            return Task.FromResult(true);
        });
    }

    public async Task<Entity?> GetEntityByIdAsync(string id)
    {
        return await Task.FromResult(_entities.Find(e => e.Id == id));
    }

    public async Task<IEnumerable<Entity>> GetAllEntitiesAsync()
    {
        return await Task.FromResult(_entities);
    }

    private async Task<bool> RetryWithBackoffAsync(Func<Task<bool>> operation)
    {
        var maxAttempts = 3;
        var delayMs = 1000;
        var success = false;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                logger.LogInformation($"Attempting operation, attempt {attempt}");
                success = await operation();
                if (success)
                    return true;
            }
            catch (Exception ex)
            {
                logger.LogError($"Operation failed, attempt {attempt}, error: {ex.Message}");
            }

            await Task.Delay(delayMs);
            delayMs *= 2;
        }

        logger.LogError($"Operation failed after {maxAttempts} attempts");
        return false;
    }
}