# 🚀 Real-time Chat App with Clean Architecture

<div align="center">
  <img src="/api/placeholder/120/120" alt="Chat Application Logo"/>
  
  [![.NET Core](https://img.shields.io/badge/.NET%20Core-8.0-blue.svg)](https://dotnet.microsoft.com/download)
  [![SignalR](https://img.shields.io/badge/SignalR-✓-brightgreen.svg)](https://dotnet.microsoft.com/apps/aspnet/signalr)
  [![Clean Architecture](https://img.shields.io/badge/Clean%20Architecture-✓-brightgreen.svg)]()
  [![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

  A modern, scalable real-time chat application built with Clean Architecture principles using .NET Core and SignalR
</div>

## ✨ Key Features

- 💬 Real-time messaging with minimal latency
- 👥 Support for both group and private chats
- 🔐 Secure user authentication and authorization
- 📱 Responsive design across all devices
- 🌐 RESTful API for third-party integration
- 📝 Message history and persistence
- ⚡ WebSocket with automatic transport fallback
- 🔍 Real-time message search
- 📸 File and image sharing capabilities
- 🔔 Push notifications

## 🏗️ Architecture Overview

The project follows Clean Architecture principles with the following layers:

### Core Layer
- **Domain**
  - Entities
  - Aggregates
  - Value Objects
  - Domain Events
  - Interfaces

- **Application**
  - Interfaces
  - DTOs
  - Commands/Queries (CQRS)
  - Command/Query Handlers
  - Domain Event Handlers
  - Exceptions
  - Behaviors/Pipelines

### Infrastructure Layer
- **Persistence**
  - DbContext
  - Repositories Implementation
  - Migrations
  - Database Configurations

- **Infrastructure**
  - External Services Implementation
  - Identity Services
  - File Storage
  - Message Brokers
  - Logging
  - Email Service

### Presentation Layer
- **API**
  - Controllers
  - SignalR Hubs
  - Filters
  - Middleware
  - API Models
  - Health Checks

- **Web**
  - Blazor Components
  - Pages
  - Shared Components
  - Services
  - View Models

## 🛠️ Technology Stack

### Backend
<div align="center">
  <img src="/api/placeholder/80/80" alt=".NET Core Logo"/> 
  <img src="/api/placeholder/80/80" alt="SignalR Logo"/>
  <img src="/api/placeholder/80/80" alt="Entity Framework Logo"/>
  <img src="/api/placeholder/80/80" alt="SQL Server Logo"/>
</div>

- **.NET 6.0** - Modern, cross-platform framework
- **SignalR Core** - Real-time communications
- **Entity Framework Core** - ORM for database operations
- **MediatR** - Mediator pattern implementation for CQRS
- **FluentValidation** - Request validation
- **AutoMapper** - Object-object mapping
- **SQL Server** - Data persistence
- **JWT Authentication** - Secure authentication
- **xUnit** - Unit testing
- **Moq** - Mocking framework
- **Swagger** - API documentation
- **Serilog** - Structured logging

### Frontend (Blazor)
<div align="center">
  <img src="/api/placeholder/80/80" alt="Blazor Logo"/>
  <img src="/api/placeholder/80/80" alt="CSS3 Logo"/>
  <img src="/api/placeholder/80/80" alt="Bootstrap Logo"/>
</div>

- **Blazor WebAssembly** - Client-side web UI
- **SignalR Client** - Real-time client communication
- **Bootstrap 5** - Responsive design framework
- **Blazored Libraries** - Common Blazor utilities
- **xUnit** - Frontend testing

## 📁 Project Structure

```plaintext
src/
├── Core/
│   ├── Domain/
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   ├── Events/
│   │   ├── Exceptions/
│   │   └── Interfaces/
│   │
│   └── Application/
│       ├── Common/
│       ├── Commands/
│       ├── Queries/
│       ├── Events/
│       ├── Interfaces/
│       ├── Models/
│       └── Behaviors/
│
├── Infrastructure/
│   ├── Persistence/
│   │   ├── Configurations/
│   │   ├── Migrations/
│   │   ├── Repositories/
│   │   └── Context/
│   │
│   └── Infrastructure/
│       ├── Services/
│       ├── Identity/
│       ├── Logging/
│       └── FileStorage/
│
└── Presentation/
    ├── API/
    │   ├── Controllers/
    │   ├── Hubs/
    │   ├── Filters/
    │   └── Middleware/
    │
    └── Web/
        ├── Pages/
        ├── Components/
        ├── Services/
        └── Models/

tests/
├── Domain.UnitTests/
├── Application.UnitTests/
├── Infrastructure.UnitTests/
└── API.IntegrationTests/
```

## 🚀 Getting Started

### Prerequisites

```bash
# Required installations
- .NET 6.0 SDK or later
- SQL Server 2019 or later
- Visual Studio 2022 or VS Code
```

### Installation

1. Clone the repository
```bash
git clone https://github.com/yourusername/realtime-chat.git
cd realtime-chat
```

2. Restore solution dependencies
```bash
dotnet restore
```

3. Update database connection string in `appsettings.json` in both API and Web projects
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ChatDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

4. Apply database migrations
```bash
cd src/Infrastructure/Persistence
dotnet ef database update
```

### Running the Application

```bash
# Run API (from src/Presentation/API directory)
dotnet run

# Run Web (from src/Presentation/Web directory)
dotnet run
```

Visit `https://localhost:5001` to view the application.

## 📖 API Documentation

API documentation is available via Swagger UI at `https://localhost:5001/swagger`

### Key API Endpoints

```plaintext
POST /api/v1/auth/login        - User authentication
POST /api/v1/auth/register     - User registration
GET  /api/v1/messages         - Retrieve message history
POST /api/v1/messages         - Send new message
GET  /api/v1/users           - Get online users
```

## 🔌 SignalR Hub Methods

```csharp
SendMessage(Message message)          - Send message to users/groups
JoinGroup(string groupName)           - Join a chat group
LeaveGroup(string groupName)          - Leave a chat group
UserTyping(string userId)             - Broadcast typing status
```

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/Domain.UnitTests
dotnet test tests/Application.UnitTests
dotnet test tests/Infrastructure.UnitTests
dotnet test tests/API.IntegrationTests
```

## 🔍 Design Patterns Used

- **Clean Architecture** - Main architectural pattern
- **CQRS** - Command Query Responsibility Segregation
- **Mediator** - For handling commands and queries
- **Repository** - Data access abstraction
- **Unit of Work** - Transaction management
- **Specification** - For query specifications
- **Factory** - For complex object creation
- **Builder** - For complex object construction
- **Decorator** - For cross-cutting concerns
- **Observer** - For domain events

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details

## 👥 Contact

Thanh An - [@thanhan.demon26](https://www.facebook.com/thanhan.demon26/)

Project Link: [https://github.com/thanhanDemon2003/ChatApp](https://github.com/thanhanDemon2003/ChatApp)

---

<div align="center">
  <sub>Built with ❤️ by [Thanh An]</sub>
</div>
