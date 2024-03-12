namespace KYC360MockAPI.Controllers;

using Repositories;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class EntityController(IEntityRepository entityRepository) : ControllerBase
{
    // GET /entity
    [HttpGet]
    public IEnumerable<Entity?> GetEntities()
    {
        return entityRepository.GetAllEntities();
    }

    // GET /entity/{id}
    [HttpGet("{id}")]
    public ActionResult<Entity> GetEntity(string id)
    {
        var entity = entityRepository.GetEntityById(id);
        return entity == null ? NotFound() : entity;
    }

    // POST /entity
    [HttpPost]
    public ActionResult<Entity> CreateEntity(Entity? entity)
    {
        var existingEntity = entityRepository.GetEntityById(entity.Id);
        if (existingEntity != null)
        {
            return StatusCode(400, "Already Exists");
        }         
        
        entityRepository.AddEntity(entity);

        return GetEntity(entity.Id);
    }

    // PUT /entity/{id}
    [HttpPut("{id}")]
    public IActionResult UpdateEntity(string id, Entity? entity)
    {
        var existingEntity = entityRepository.GetEntityById(id);
        if (existingEntity == null)
        {
            return NotFound();
        }

        entityRepository.UpdateEntity(entity);

        return Ok("Updated Sucessfully!");
    }

    // DELETE /entity/{id}
    [HttpDelete("{id}")]
    public IActionResult DeleteEntity(string id)
    {
        var entity = entityRepository.GetEntityById(id);
        if (entity == null)
        {
            return NotFound();
        }

        entityRepository.DeleteEntity(id);

        return Ok("Deleted Sucessfully");
    }

    // GET /entity/search
    [HttpGet("search")]
    public IEnumerable<Entity?> SearchEntities([FromQuery] string search)
    {
        return entityRepository.GetAllEntities()
            .Where(e =>
                e!.Names.Any(n =>
                    $"{n.FirstName} {n.MiddleName} {n.Surname}".Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                e.Addresses!.Any(a =>
                    $"{a.AddressLine} {a.City} {a.Country}".Contains(search, StringComparison.OrdinalIgnoreCase)));
    }

    // GET /entity/filter
    [HttpGet("filter")]
    public IEnumerable<Entity?> FilterEntities(
        [FromQuery] string gender,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] string[] countries)
    {
        var filteredEntities = entityRepository.GetAllEntities();

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

        if (countries.Length > 0)
        {
            filteredEntities = filteredEntities.Where(e =>
                e.Addresses.Any(a => countries.Contains(a.Country)));
        }

        return filteredEntities;
    }
}
