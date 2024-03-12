# DOCUMENTATION

This is an exercise building a REST API endpoints serving Entity data from a mocked database.
The API provides endpoints to manage entities with basic information such as names, addresses and dates. It includes classes for entities, addresses, dates and names. As well as a repository interface (IEntityRepository) and a mock implementation (MockEntityRepository). The API also includes a controller (EntityController) with endpoints for CRUD operations on entities and endpoints for searching and filtering entities.
## Classes
### Entity
Implements the `IEntity` interface and provides details of an entity.
- ID : Unique identifier for the entity
- Deceased : Indicates if the entity is deceased
- Gender : Gender of the entity
- Addresses : List of addresses of the entity
- Dates : Lists of dates of the entity
- Names : List of names of the entity
### Address
Represents an address.
- AddressLine : Street address.
- City : City
- Country : Country
### Date
Represents a date.
- DateType : Type of date (e.g., Birth)
- DateValue : Date value
### Name
Represents a name.
- FirstName : First name
- MiddleName : Middle name
- Surname : Surname
### MockEntityRepository
Implements the `IEntityRepository` interface and provides mock data for entities.
- GenerateMockData(): Generates mock data for entities using the Bogus library.
- GetAllEntities(): Gets all entities.
- GetEntityById(string id): Gets an entity by ID.
- AddEntity(Entity entity): Adds a new entity.
- UpdateEntity(Entity entity): Updates an existing entity.
- DeleteEntity(string id): Deletes an entity by ID.

## Interfaces
### IEntity
Represents an entity.
- Id: Unique identifier for the entity.
- Deceased: Indicates if the entity is deceased.
- Gender: Gender of the entity.
- Addresses: List of addresses of the entity.
- Dates: List of dates of the entity.
- Names: List of names of the entity.
### IEntityRepository
Defines methods for managing entities.
- GetAllEntities(): Gets all entities.
- GetEntityById(string id): Gets an entity by ID.
- AddEntity(Entity entity): Adds a new entity.
- UpdateEntity(Entity entity): Updates an existing entity.
- DeleteEntity(string id): Deletes an entity by ID.

## Controllers
### EntityController 
Implements CRUD endpoints for entities and additional endpoints for searching and filtering entities.

## Endpoints
### GET /entity
Returns a list of all entities.
### GET /entity/{id}
Returns the entity with the specified ID.
### POST /entity
Creates a new entity.
### PUT /entity/{id}
Updates an existing entity with the specified ID.
### DELETE /entity/{id}
Deletes the entity with the specified ID.
### GET /entity/search
Searches for entities based on a search query.

Query Parameters:
search: Search query.
### GET /entity/filter
Filters entities based on specified criteria.

Query Parameters:
- gender: Gender of the entity.
- startDate: Start date for filtering.
- endDate: End date for filtering.
- countries: Array of countries for filtering.

## Reasoning Behind the Approach
### Organization
Usage of classes and separation of repositories and the controller for easy maintenance and testing environment.
### Dependency Injection
Use of dependency injection makes it easier to switch up from using mock data to real database without much effort. 
### Bogus Library for Mock Data
Bogus library is used to generate realistic mock data for the entities.
### RESTful API Design
The API follows the RESTful principles, with the endpoints named according to their usage (`GET /entity`, `POST /entity`) making it intuitive and easy to use.
### Query Parameters for Searching and Filtering
The API includes query parameters for searching and filtering entities (`GET /entity/search`, `GET /entity/filter`), providing flexibility and allowing clients to retrieve specific subsets of data.

Overall, the approach aims to provide a well-structured and flexible mock API with realistic mock data and following the RESTful principles.