# ShopFlow - E-commerce Platform

## Overview
ShopFlow is a modern e-commerce platform built with .NET 9 and Clean Architecture principles. The project follows a database-first approach and is designed to be scalable, maintainable, and testable.

## Architecture

### Clean Architecture Layers

1. **Domain Layer** (`ShopFlow.Domain`)
   - Entities (Business objects)
   - Value Objects
   - Domain Services
   - Specifications

2. **Application Layer** (`ShopFlow.Application`)
   - Use Cases
   - DTOs/Contracts
   - Repository Interfaces
   - Service Interfaces
   - Validation

3. **Infrastructure Layer** (`ShopFlow.Infrastructure`)
   - Repository Implementations
   - Service Implementations
   - Database Context
   - External Service Integrations
   - Mappers

4. **API Layer** (`ShopFlow.API`)
   - Controllers
   - Middleware
   - Filters
   - API Configuration

## Getting Started

### Prerequisites
- .NET 9 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Database Setup
1. Ensure SQL Server is running
2. Update connection string in `appsettings.json`
3. The application uses database-first approach, so the database schema should already exist

### Running the Application
1. Clone the repository
2. Navigate to the solution directory
3. Restore NuGet packages:
   ```bash
   dotnet restore
   ```
4. Build the solution:
   ```bash
   dotnet build
   ```
5. Run the API project:
   ```bash
   cd ShopFlow.API
   dotnet run
   ```

### API Endpoints

#### Users
- `POST /api/users` - Create a new user
- `GET /api/users/{id}` - Get user by ID

#### Products
- `POST /api/products` - Create a new product
- `GET /api/products/{id}` - Get product by ID

## Project Structure

```
ShopFlow/
├── ShopFlow.Domain/           # Domain entities and business logic
├── ShopFlow.Application/       # Application services and use cases
├── ShopFlow.Infrastructure/   # Data access and external services
├── ShopFlow.API/             # Web API controllers and configuration
└── Tests/                    # Unit and integration tests
```

## Key Features

- **Clean Architecture**: Separation of concerns with clear layer boundaries
- **Repository Pattern**: Abstracted data access layer
- **Unit of Work**: Transaction management
- **Value Objects**: Immutable objects for domain concepts
- **Global Exception Handling**: Consistent error responses
- **Validation**: Input validation using Data Annotations
- **Swagger Documentation**: API documentation and testing

## Database Schema

The application includes entities for:
- Users and authentication
- Products and categories
- Inventory management
- Order processing
- Payment transactions
- Shipping and logistics

## Contributing

1. Follow Clean Architecture principles
2. Write unit tests for business logic
3. Use meaningful commit messages
4. Follow C# coding conventions

## License

This project is licensed under the MIT License.
