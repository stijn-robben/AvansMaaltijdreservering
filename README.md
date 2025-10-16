<div align="center">
  <img src="https://i.ibb.co/WW93TtxG/front-img.jpg" alt="Avans Meal Rescue " width="400"/>

  # Avans Meal Rescue Platform

  ### Combating Food Waste Through Technology

  [![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
  [![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-MVC-512BD4)](https://docs.microsoft.com/aspnet/core)
  [![Azure](https://img.shields.io/badge/Azure-Deployed-0078D4?logo=microsoft-azure)](https://azure.microsoft.com/)
  [![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Code%20First-512BD4)](https://docs.microsoft.com/ef/)
  [![License](https://img.shields.io/badge/License-Educational-green.svg)](LICENSE)

</div>

---

## 👨‍💻 About This Project

This application was developed by **Stijn Robben**, a Computer Science student at Avans University of Applied Sciences, as part of the course **Server-Side Web Development Individual (EIIN-SSWPI)**.

The project demonstrates a full-stack enterprise application built with modern .NET technologies, implementing clean architecture principles, RESTful APIs, and a complete CI/CD pipeline.

### 🎯 Project Goal

Avans Meal Rescue is a web-based platform designed to combat food waste at Avans University. Similar to "Too Good To Go," the system allows canteen employees to offer surplus food packages at reduced prices, which students can reserve and pick up before closing time.

---

## ✨ Key Features

### For Students
- 📦 **Browse Available Packages** - View all surplus food packages from different canteen locations
- 🔍 **Smart Filtering** - Filter by location (Breda, Tilburg, Den Bosch) and meal type
- 🛒 **One-Click Reservations** - Reserve packages instantly with real-time availability
- 🔞 **Age Verification** - Automatic 18+ validation for packages containing alcohol
- 📱 **Responsive Design** - Full mobile and desktop support
- 🚫 **No-Show Protection** - Fair system preventing abuse (max 2 no-shows)

### For Canteen Employees
- ➕ **Package Management** - Create, edit, and delete food packages
- 👥 **Reservation Overview** - View all reservations with student contact details
- ⚠️ **No-Show Registration** - Track students who don't pick up their packages
- 📊 **Multi-Canteen View** - See offerings from all Avans canteens
- ⏰ **Time-Based Controls** - Packages limited to max 2 days in advance

### Technical Highlights
- 🏗️ **Onion Architecture** - Clean separation of concerns with Domain-Driven Design
- 🔐 **ASP.NET Identity** - Secure authentication and role-based authorization
- 🌐 **RESTful API (RMM Level 2)** - Full CRUD operations with proper HTTP verbs
- 📡 **GraphQL Endpoint** - Flexible data querying for mobile clients
- 🔒 **Thread-Safe Reservations** - Race condition prevention with locking mechanism
- 🧪 **Comprehensive Testing** - Unit tests with Moq + Postman E2E tests
- 🚀 **CI/CD Pipeline** - Automated builds, tests, and deployments to Azure

---

## 📸 Screenshots

### Student Dashboard
![Student Dashboard](https://i.ibb.co/nMRWK4V7/student-dashboard.jpg)
*Students can browse available packages with filters for location and meal type*

### Package Details
![Package Details](https://i.ibb.co/GQV86ytb/package-details.jpg)
*Detailed view showing example products with photos and 18+ indicators*

### Employee Dashboard
![Employee Dashboard](https://i.ibb.co/HpP2ZZMj/employee-dashboard.jpg)
*Canteen employees can manage packages and track reservations*

### API Documentation (Swagger)
![Swagger API](https://i.ibb.co/yFLbYMJ1/swagger.png)
*Interactive API documentation for all REST endpoints*

---

## 🏗️ Architecture

This project implements **Clean Architecture (Onion Architecture)** with strict dependency rules:

### Package Diagram
![Package Diagram](https://i.ibb.co/G438zNs0/Package-Diagram.jpg)

### Class Diagram
![Class Diagram](https://i.ibb.co/6qjyK0P/Class-Diagram.jpg)

### Deployment Diagram
![Deployment Diagram](https://i.ibb.co/RGYmXfXS/Deployment-Diagram.jpg)

### Project Structure
```
AvansMaaltijdreservering/
├── Core.Domain/              # Domain entities and interfaces (no dependencies)
├── Core.DomainService/       # Business logic and domain services
├── Infrastructure/           # Data access, repositories, Identity
│   ├── Data/                 # EF Core DbContext & migrations
│   ├── Repositories/         # Repository implementations
│   └── Identity/             # ASP.NET Identity configuration
├── API/                      # RESTful Web API + GraphQL
│   ├── Controllers/          # REST API endpoints
│   ├── GraphQL/              # GraphQL queries and mutations
│   └── DTOs/                 # Data transfer objects
├── WebApp/                   # ASP.NET MVC Web Application
│   ├── Controllers/          # MVC controllers
│   ├── Views/                # Razor views
│   └── ViewModels/           # View models
└── Tests/
    ├── Core.Domain.Tests/    # Domain unit tests
    └── Infrastructure.Tests/ # Repository tests
```

---

## 🛠️ Technology Stack

### Backend
- **Framework:** ASP.NET Core 9.0 MVC
- **Language:** C# 12
- **ORM:** Entity Framework Core 9.0 (Code First)
- **Database:** Azure SQL Database (2 databases: Main + Identity)
- **Authentication:** ASP.NET Core Identity with JWT tokens
- **API:** REST (RMM Level 2) + GraphQL (HotChocolate)

### Frontend
- **UI Framework:** Bootstrap 5
- **View Engine:** Razor Pages
- **JavaScript:** Vanilla JS for interactive features
- **Responsive Design:** Mobile-first approach

### DevOps & Cloud
- **Cloud Platform:** Microsoft Azure
  - App Services (WebApp + API)
  - Azure SQL Database
  - Application Insights
- **CI/CD:** Azure DevOps Pipelines
- **Version Control:** Git + GitHub
- **API Testing:** Postman with automated collections

### Testing
- **Unit Testing:** xUnit
- **Mocking:** Moq
- **E2E Testing:** Postman

### Architecture Patterns
- **Clean Architecture (Onion Architecture)**
- **Repository Pattern**
- **Dependency Injection**
- **SOLID Principles**

---

## 🚀 CI/CD Pipeline

![Azure DevOps Pipeline](https://i.ibb.co/HfHMT8FL/devops.jpg)

The project uses Azure DevOps for continuous integration and deployment:

1. **Build Stage**
   - Restore NuGet packages
   - Compile solution (0 warnings policy)
   - Run unit tests with code coverage

2. **Test Stage**
   - Execute xUnit tests
   - Validate business rules
   - Check code quality

3. **Deploy Stage**
   - Deploy WebApp to Azure App Service
   - Deploy API to Azure App Service
   - Run EF Core migrations on Azure SQL Database
   - Update Application Insights

All stages run automatically on every commit to `master` branch.

---

## 📋 Implemented User Stories

### Mandatory (7/7 ✅)
- ✅ **US_01:** Student package overview (available + reserved)
- ✅ **US_02:** Canteen employee dashboard (own + all canteens)
- ✅ **US_03:** Package CRUD operations with 2-day limit
- ✅ **US_04:** Automatic 18+ marking for alcohol products
- ✅ **US_05:** Student reservations (max 1 per day)
- ✅ **US_06:** Product information with photos and disclaimer
- ✅ **US_07:** Race condition prevention (first-come-first-served)

### Extra Features (2/3 ✅)
- ✅ **US_08:** Filter by location and meal type
- ✅ **US_10:** No-show registration with automatic blocking (2+ no-shows)

### Technical Requirements (100% ✅)
- ✅ Onion Architecture
- ✅ Repository Pattern with interfaces
- ✅ Entity Framework Code First + Migrations
- ✅ ASP.NET Identity (separate database)
- ✅ RESTful API (Richardson Maturity Model Level 2)
- ✅ GraphQL endpoint
- ✅ Unit tests with Moq
- ✅ Postman E2E tests
- ✅ Azure deployment
- ✅ CI/CD pipeline
- ✅ No build warnings
- ✅ Thread-safe operations

---

## 🔑 API Endpoints

### REST API (RMM Level 2)

#### Packages
```http
GET    /api/packages              # Get all packages
GET    /api/packages/{id}         # Get package by ID
POST   /api/packages              # Create new package
PUT    /api/packages/{id}         # Update package
DELETE /api/packages/{id}         # Delete package
```

#### Reservations
```http
POST   /api/reservations          # Create reservation
DELETE /api/reservations/{id}     # Cancel reservation
GET    /api/reservations/student/{id}  # Get student reservations
```

#### Students
```http
GET    /api/students/{id}         # Get student by ID
POST   /api/students              # Register new student
```

### GraphQL Endpoint

**Endpoint:** `/graphql`

**Example Query:**
```graphql
query {
  packages {
    id
    name
    price
    pickupDateTime
    canteenLocation
    products {
      name
      containsAlcohol
      photoUrl
    }
  }
}
```

**Example Mutation:**
```graphql
mutation {
  createReservation(packageId: 1, studentId: 1) {
    id
    reservedByStudent {
      name
      email
    }
  }
}
```

📖 **API Documentation available via Swagger UI at `/swagger` endpoint**

---

## 🧪 Testing

### Unit Tests
Located in `AvansMaaltijdreservering.Core.Domain.Tests/`

**Coverage:**
- ✅ Package business rules (18+ marking, product validation)
- ✅ Student validation (age restrictions, no-show blocking)
- ✅ Reservation logic (1 per day, duplicate prevention)
- ✅ Custom validation attributes

**Run tests:**
```bash
dotnet test
```

### E2E Tests
Postman collections in `/Rest/` directory:
- `endpoint-tests-azure.postman_collection.json` - Azure deployment
- `endpoint-tests.postman_collection.json` - Local development

**Features:**
- Automated JWT token management
- Full CRUD workflow tests
- Error scenario validation
- GraphQL query testing

---

## 💻 Local Development

### Prerequisites
- .NET 9.0 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 / VS Code / Rider
- Git

### Setup

1. **Clone repository**
```bash
git clone https://github.com/stijnrobben/AvansMaaltijdreservering.git
cd AvansMaaltijdreservering
```

2. **Update connection strings**

Edit `appsettings.Development.json` in both API and WebApp projects:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AvansMeals;Trusted_Connection=True;",
    "IdentityConnection": "Server=(localdb)\\mssqllocaldb;Database=AvansMealsIdentity;Trusted_Connection=True;"
  }
}
```

3. **Run migrations**
```bash
cd AvansMaaltijdreservering.Infrastructure
dotnet ef database update --context ApplicationDbContext
dotnet ef database update --context ApplicationIdentityDbContext
```

4. **Seed database**

Run the SQL scripts in `/Rest/` directory:
- `DatabaseSeed.sql` - Test data (canteens, products, packages)

5. **Run applications**

**Option A: Visual Studio**
- Set multiple startup projects (API + WebApp)
- Press F5

**Option B: CLI**
```bash
# Terminal 1 - API
cd AvansMaaltijdreservering.API
dotnet run

# Terminal 2 - WebApp
cd AvansMaaltijdreservering.WebApp
dotnet run
```

**Local URLs:**
- WebApp: https://localhost:7001
- API: https://localhost:7002
- Swagger: https://localhost:7002/swagger
- GraphQL: https://localhost:7002/graphql

---

## 📦 Database Schema

### Main Database (ApplicationDbContext)
- **Students** - Student accounts and no-show tracking
- **CanteenEmployees** - Employee accounts linked to canteens
- **Canteens** - Canteen locations and capabilities
- **Packages** - Food packages with pricing and availability
- **Products** - Individual food items with photos
- **PackageProducts** - Many-to-many relationship

### Identity Database (ApplicationIdentityDbContext)
- **AspNetUsers** - User credentials (linked to Students/Employees)
- **AspNetRoles** - Roles (Student, CanteenEmployee)
- Standard ASP.NET Identity tables

---

## 🔒 Security Features

- ✅ **ASP.NET Identity** - Secure password hashing (PBKDF2)
- ✅ **Role-Based Authorization** - Separate student/employee permissions
- ✅ **Anti-Forgery Tokens** - CSRF protection on all forms
- ✅ **JWT Authentication** - Secure API access
- ✅ **SQL Injection Prevention** - Entity Framework parameterized queries
- ✅ **HTTPS Enforcement** - All traffic encrypted
- ✅ **Separate Identity Database** - Credentials isolated from business data
- ✅ **No Credentials in Code** - Connection strings in Azure Configuration

---

## 🎓 Learning Outcomes

This project demonstrates proficiency in:

1. **Software Architecture**
   - Clean Architecture / Onion Architecture
   - Dependency Inversion Principle
   - Separation of Concerns

2. **Backend Development**
   - ASP.NET Core MVC
   - Entity Framework Core (Code First)
   - Repository Pattern
   - Domain-Driven Design
   - Business Rule Validation

3. **API Development**
   - RESTful API design (RMM Level 2)
   - GraphQL schema design
   - API documentation (Swagger/OpenAPI)
   - Proper HTTP status codes

4. **Database Design**
   - Relational database modeling
   - Many-to-many relationships
   - Migrations and version control
   - Database normalization

5. **Testing**
   - Unit testing with xUnit
   - Mocking with Moq
   - Test-Driven Development
   - E2E testing with Postman

6. **DevOps**
   - Azure App Services deployment
   - CI/CD pipelines
   - Automated testing in pipeline
   - Database migration automation

7. **Security**
   - Authentication & Authorization
   - Identity management
   - Secure coding practices

8. **UX Design**
   - Responsive web design
   - User-centered design
   - Accessibility considerations

---

