# TransactionApp

A simple Transaction Management API built with C# and .NET, following N-Tier layered architecture principles.

## Overview

TransactionApp is a RESTful API that enables 
- CRUD operations for USER entities
- Adding financial transactions for USERS
- Filtering high-volume transactions
- Viewing meaningful transaction data such as transaction per user or per transaction type. 

## Architecture

This project follows the **N-Tier Layered Architecture** pattern, ensuring separation of concerns and maintainability:

```
TransactionApp
├── TransactionApp.API          # Presentation Layer (Controllers, Endpoints)
├── TransactionApp.BUSINESS     # Business Logic Layer (Services, Calculations)
├── TransactionApp.DAL          # Data Access Layer (Database Context, Repositories)
├── TransactionApp.ENTITIES     # Domain Models (Transaction, User entities)
└── TransactionApp.CORE         # Cross-cutting Concerns (Middleware, Utilities)
```

### Project Structure

- **TransactionApp.API**: The entry point of the application containing API controllers, endpoint definitions, and configuration.

- **TransactionApp.BUSINESS**: Contains business logic including:
  - CRUD operations for Users
  - Transaction management services
  - Business calculations (e.g., total transactions per user)
  - Validation and business rules

- **TransactionApp.DAL**: Data Access Layer responsible for:
  - Database context configuration
  - Repository pattern implementation
  - Data persistence operations

- **TransactionApp.ENTITIES**: Domain models including:
  - `User`: User entity with properties and relationships
  - `Transaction`: Transaction entity linked to users
  - Entity relationships and data annotations

- **TransactionApp.CORE**: Shared components used across the application:
  - Exception middleware for centralized error handling
  - Common utilities and helpers

## Features

- ✅ User management (Create, Read, Update, Delete)
- ✅ Transaction management with user association
- ✅ Calculate total transactions per user / per transactions
- ✅ Show high threshold transactions
- ✅ In-memory transaction calculation
- ✅ Code first approach with FluentApi
- ✅ RESTful API design
- ✅ Centralized exception handling
- ✅ N-Tier architecture for better code organization

## Technologies Used

- **Framework**: .NET / ASP.NET Core
- **Language**: C#
- **Architecture**: N-Tier Layered Architecture
- **Design Patterns**: Repository Pattern, Unit Of Work, Dependency Injection, Result Pattern

## Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (version 6.0 or later)
- A database system (SQL Server, PostgreSQL, or SQLite)
- An IDE (Visual Studio, VS Code, or Rider)

### Installation

1. Clone the repository:
```bash
git clone https://github.com/YasinEmreEvirgen/TransactionApp.git
cd TransactionApp
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Update the connection string in `appsettings.json` (located in TransactionApp.API):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your_Connection_String_Here"
  }
}
```

4. Apply database migrations:
```bash
dotnet ef database update --project TransactionApp.DAL --startup-project TransactionApp.API
```

5. Run the application:
```bash
dotnet run --project TransactionApp.API
```

The API will be available at `https://localhost:5001` (or the port specified in your launch settings).

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the project
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is open source and available under the [MIT License](LICENSE).

## Contact

Yasin Emre Evirgen - [@YasinEmreEvirgen](https://github.com/YasinEmreEvirgen)

Project Link: [https://github.com/YasinEmreEvirgen/TransactionApp](https://github.com/YasinEmreEvirgen/TransactionApp)

## Acknowledgments

- Built with ASP.NET Core
- Follows N-Tier architecture principles
