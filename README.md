# DOCUMENTATION

This is an exercise building a REST API endpoints serving Entity data from a mocked database.
The API provides endpoints to manage entities with basic information such as names, addresses and dates. It includes classes for entities, addresses, dates and names. As well as a repository interface (IEntityRepository) and a mock implementation (MockEntityRepository). Also including database interface (IDatabase) alongside it's mock implementation (MockDatabase) with logging, retry and error handling. The API also includes a controller (EntityController) with endpoints for CRUD operations on entities and endpoints for searching and filtering entities. Test Cases for common scenarios are also contained in Tests/EntityControllerTests with mock entities generation using Bogus library in Tests/MockDatabaseGenerator.
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
### Gender
An enum representing the gender of an entity.
### MockEntityRepository
Mock implementation of `IEntityRepository` using the mock database for CRUD operations.
- AddEntityAsync(Entity entity): Add an entity to the database.
- UpdateEntityAsync(Entity entity): Update an entity.
- DeleteEntityAsync(string id): Delete an entity by ID.
- GetEntityByIdAsync(string id): Retrieve entity by ID.
- GetAllEntitiesAsync(): Retrieve all entities.
### MockDatabase
Mock implementation of `IDatabase`. It stores entities in memory and fully provides the CRUD operations with retry and backoff strategy along with logging each operation.
- AddEntityAsync(Entity entity): Add an entity to the database.
- UpdateEntityAsync(Entity entity): Update an entity.
- DeleteEntityAsync(string id): Delete an entity by ID.
- GetEntityByIdAsync(string id): Retrieve entity by ID.
- GetAllEntitiesAsync(): Retrieve all entities.

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
- AddEntityAsync(Entity entity): Add an entity to the database.
- UpdateEntityAsync(Entity entity): Update an entity.
- DeleteEntityAsync(string id): Delete an entity by ID.
- GetEntityByIdAsync(string id): Retrieve entity by ID.
- GetAllEntitiesAsync(): Retrieve all entities.
### IDatabase
Defines methods defining CRUD operations on entities.
- AddEntityAsync(Entity entity): Add an entity to the database.
- UpdateEntityAsync(Entity entity): Update an entity.
- DeleteEntityAsync(string id): Delete an entity by ID.
- GetEntityByIdAsync(string id): Retrieve entity by ID.
- GetAllEntitiesAsync(): Retrieve all entities.

## Services
### MockDataGenerator
Responsible for generating mock entities with fake data for testing purposes.
## Controllers
### EntityController 
Implements CRUD endpoints for entities and additional endpoints for searching and filtering entities.

## Endpoints
### GET /entity
Returns a list of all entities.

Query Parameters:
- page : Current page number. Default = 1.
- pageSize : Number of entries per page. Default = 10.
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
- search: Search query.
- page : Current page number. Default = 1.
- pageSize : Number of entries per page. Default = 10.
### GET /entity/filter
Filters entities based on specified criteria.

Query Parameters:
- gender: Gender of the entity.
- startDate: Start date for filtering.
- endDate: End date for filtering.
- countries: Array of countries for filtering.
- page : Current page number. Default = 1.
- pageSize : Number of entries per page. Default = 10.
### GET /entity/generate
Generates mock entities for given count.

Query Parameters:
- count: The number of mock entities to be generated.

## Tests
### EntityControllerTests
Unit tests for `EntityController` to verify the behavior of its actions.
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
### Rationale for Retry and Backoff Strategy
The retry and backoff strategy is implemented in the `MockDatabase` class for handling transient failures during database operations. 

#### Chosen method
- Exponential Backoff : The delay between retry attempts doubles after each unsuccessful attempt. This prevents the system from overwhelming itself with excessive retry attempts.
- Limited Retries : Limit the number of retry attempts prevents the system from having indefinitely retrying failed operations. This makes sure the system doesn't waste resource in a indefinite retry loop.
- Logging: Relevant information about each retry attempt, operation being retried and number of attempts made are logged. Logging helps in monitoring and troubleshooting unexpected behavior.  
#### Rationale behind the choice:
- System Stability : Implementing a retry and backoff strategy improves the system stability by automatically attempting to recover from temporary failures due to network issues, database unavailability or insufficient system resource.
- User Experience : Handling temporary failures without exposing the users to errors helps ensuring the users have a seamless experience. The backoff strategy also prevents excessive load on the system which further enhances the user experience.
- Nature of Potential Transient Failures: Transient failures are often caused by temporary issues such as spikes in traffic, or momentary unavailability of system resources. These type of failures often resolve themselves after a short period. Using a retry and backoff strategy is well suited for handling such transient failures. It makes sure the system retry failed operations after a short period, giving time for the infrastructure to recover.

Conclusion: The chosen retry and backoff strategy aims to enhances system stability, user experience and effectively handle transient failures. The approach lessen the impact of temporary issues while avoiding excessive resource consumption.

This documentation provides a comprehensive overview of the project, its components and the rationale behind the implementation of the retry and backoff strategy. It helps developers understand the purpose and design decisions of the codebase.