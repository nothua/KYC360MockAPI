using KYC360MockAPI.Services;

namespace KYC360MockAPI.Controllers;

using Repositories;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class EntityController(IEntityRepository entityRepository, ILogger<EntityController> logger) : ControllerBase
{
    // GET /entity
    [HttpGet]
    public async Task<IActionResult>  GetEntities([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string sortBy = "Id")
    {
        try{
            var entities = await entityRepository.GetAllEntitiesAsync();

            entities = sortBy.ToLower() switch
            {
                "deceased" => entities.OrderBy(e => e.Deceased),
                "gender" => entities.OrderBy(e => e.Gender),
                _ => entities.OrderBy(e => e.Id)
            };

            var totalCount = entities.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var paginatedEntities = entities.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var response = new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                PageSize = pageSize,
                CurrentPage = page,
                Entities = paginatedEntities
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"An error occurred while retrieving entities: {ex.Message}");
            return StatusCode(500, $"An error occurred while retrieving entities: {ex.Message}");
        }
    }

    // GET /entity/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Entity>> GetEntity(string id)
    {
        try
        {
            var entity = await entityRepository.GetEntityByIdAsync(id);
            return entity == null ? NotFound() : entity;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"An error occurred while retrieving the entity with ID {id}: {ex.Message}");
            return StatusCode(500, $"An error occurred while retrieving the entity with ID {id}: {ex.Message}");
        }
    }

    // POST /entity
    [HttpPost]
    public async Task<IActionResult> CreateEntity(Entity? entity)
    {
        try
        {
            var existingEntity = await entityRepository.GetEntityByIdAsync(entity.Id);
            if (existingEntity != null)
            {
                return Conflict($"An entity with ID {entity.Id} already exists.");
            }

            await entityRepository.AddEntityAsync(entity);

            return Ok("Created Successfully!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"An error occurred while creating the entity: {ex.Message}");
            return StatusCode(500, $"An error occurred while creating the entity: {ex.Message}");
        }
    }

    // PUT /entity/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEntity(string id, Entity? entity)
    {
        try
        {
            var existingEntity = await entityRepository.GetEntityByIdAsync(id);
            if (existingEntity == null)
                return NotFound($"No entity found with ID {id}.");
            

            if (id != entity.Id)
                return BadRequest("Entity ID mismatch.");
            

            await entityRepository.UpdateEntityAsync(entity);

            return Ok("Updated Successfully!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"An error occurred while updating the entity with ID {id}: {ex.Message}");
            return StatusCode(500, $"An error occurred while updating the entity with ID {id}: {ex.Message}");
        }
    }

    // DELETE /entity/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEntity(string id)
    {
        try
        {
            var entity = await entityRepository.GetEntityByIdAsync(id);
            if (entity == null)
                return NotFound($"No entity found with ID {id}.");

            await entityRepository.DeleteEntityAsync(id);

            return Ok("Deleted Successfully!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"An error occurred while deleting the entity with ID {id}: {ex.Message}");
            return StatusCode(500, $"An error occurred while deleting the entity with ID {id}: {ex.Message}");
        }
    }

    // GET /entity/search
    [HttpGet("search")]
    public async Task<IActionResult> SearchEntities([FromQuery] string search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var allEntities = await entityRepository.GetAllEntitiesAsync();
            var filteredEntities = allEntities.Where(e =>
                e!.Names.Any(n =>
                    $"{n.FirstName} {n.MiddleName} {n.Surname}".Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                e.Addresses!.Any(a =>
                    $"{a.AddressLine} {a.City} {a.Country}".Contains(search, StringComparison.OrdinalIgnoreCase)));

            var totalCount = filteredEntities.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var paginatedEntities = filteredEntities.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var response = new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                PageSize = pageSize,
                CurrentPage = page,
                Entities = paginatedEntities
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"An error occurred while retrieving the entities with search query {search}: {ex.Message}");
            return StatusCode(500, $"An error occurred while retrieving the entities with search query {search}: {ex.Message}");
        }
    }

    // GET /entity/filter
    [HttpGet("filter")]
    public async Task<IActionResult> FilterEntities(
        [FromQuery] string gender,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] string[]? countries,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var allEntities = await entityRepository.GetAllEntitiesAsync();
            var filteredEntities = allEntities;

            if (!string.IsNullOrEmpty(gender))
            {
                filteredEntities = filteredEntities.Where(e => e!.Gender == gender);
            }

            if (startDate != null)
            {
                filteredEntities = filteredEntities.Where(e =>
                    e.Dates.Any(d => d.DateValue >= startDate));
            }

            if (endDate != null)
            {
                filteredEntities = filteredEntities.Where(e =>
                    e.Dates.Any(d => d.DateValue <= endDate));
            }

            if (countries != null && countries.Length > 0)
            {
                filteredEntities = filteredEntities.Where(e =>
                    e.Addresses.Any(a => countries.Contains(a.Country)));
            }

            var totalCount = filteredEntities.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var paginatedEntities = filteredEntities.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var response = new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                PageSize = pageSize,
                CurrentPage = page,
                Entities = paginatedEntities
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"An error occurred while retrieving the entities with the filter query\nGender : {gender}\nStart Date : {startDate}\nEnd Date: {endDate}\nCountries : {countries}\n {ex.Message}");
            return StatusCode(500, $"An error occurred while retrieving the entities with the filter query\nGender : {gender}\nStart Date : {startDate}\nEnd Date: {endDate}\nCountries : {countries}\n {ex.Message}");
        }
    }
    
    // POST /entity/generate
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateMockEntities(int count)
    {
        try
        {
            var mockEntities = MockDataGenerator.GenerateMockEntities(count);
            foreach (var entity in mockEntities)
            {
                await entityRepository.AddEntityAsync(entity);
            }

            return Ok($"Successfully generated and added {count} mock entities.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"An error occurred while generating mock entities: {ex.Message}");
            return StatusCode(500, $"An error occurred while generating mock entities: {ex.Message}");
        }
    }
}
