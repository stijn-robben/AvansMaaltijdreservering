cd /mnt/c/Users/stijn/Documents/GitHub/AvansMaaltijdreservering

# Complete Project Context - Avans Maaltijdreservering Systeem

## 🎯 CASUS OVERZICHT

**Probleem:** Avans kantines gooien dagelijks veel eten weg aan het eind van de dag.
**Oplossing:** Een "Too Good To Go" alternatief specifiek voor Avans studenten.
**Doel:** Voedselverspilling tegengaan door overschotten tegen gereduceerde prijs aan te bieden.

## 👥 ACTOREN & ROLLEN

### Student
- **Gegevens:** Naam, geboortedatum, studentnummer, e-mail, studiestad, telefoonnummer
- **Restricties:** 
  - Geboortedatum mag niet in toekomst liggen
  - Minimaal 16 jaar bij aanmelden
  - Maximaal 1 pakket per afhaaldag
  - Geen 18+ pakketten voor minderjarigen
  - Na 2+ no-shows: geen nieuwe reserveringen mogelijk

### Kantinemedewerker
- **Gegevens:** Naam, personeelsnummer, locatie (kantine waar hij/zij werkt)
- **Rechten:**
  - Pakketten aanmaken/wijzigen/verwijderen (alleen eigen locatie)
  - Overzicht alle pakketten (eigen + andere kantines)
  - No-shows registreren

## 🏢 LOCATIES & KANTINES

### Steden
- Breda
- Tilburg  
- Den Bosch

### Kantines
- Per stad meerdere kantines (via enumeratie)
- Elke kantine heeft indicatie of warme maaltijden aangeboden worden
- Kantinemedewerker gekoppeld aan specifieke kantine

## 📦 PAKKET SYSTEEM

### Pakket Eigenschappen
- **Beschrijvende naam** (verplicht, niet leeg)
- **Productenlijst** (indicatie op basis van historie - GEEN garantie!)
- **Locatie** (stad + specifieke kantine)
- **Ophaaltijd** (datum + tijd)
- **Uiterste ophaaltijd**
- **18+ indicator** (automatisch als alcoholhoudend product)
- **Prijs**
- **Type maaltijd** (brood, warme avondmaaltijd, drank, etc. - enumeratie)
- **Reservering** (referentie naar student)

### Belangrijke Business Rules
- Pakket mag max 2 dagen vooruit gepland worden
- Wijzigen/verwijderen alleen als nog geen reservering
- Warme maaltijd pakketten alleen op locaties die dit aanbieden
- Automatische 18+ marking bij alcoholhoudende producten

### Product Eigenschappen
- **Naam** (verplicht, niet leeg)
- **Alcoholhoudend** (ja/nee)
- **Foto**

## 🎯 VERPLICHTE USER STORIES (1-7) + 1 EXTRA (8, 9, OF 10)

### US_01 (3 SP) - Student Overzicht
- Twee pagina's: beschikbare pakketten + eigen reserveringen
- Duidelijke navigatie tussen beide overzichten

### US_02 (2 SP) - Kantinemedewerker Overzicht  
- Eigen kantine pakketten (gesorteerd op datum)
- Alle kantines overzicht (gesorteerd op datum)

### US_03 (3 SP) - Pakket Beheer
- CRUD operaties voor pakketten (alleen eigen locatie)
- Max 2 dagen vooruitplannen
- Alleen wijzigen/verwijderen zonder reserveringen

### US_04 (1 SP) - 18+ Restricties
- Automatische 18+ marking bij alcohol
- Leeftijdscontrole bij reservering (tov ophaaldatum)

### US_05 (3 SP) - Reservering Plaatsen
- Student kan pakket reserveren
- Max 1 pakket per afhaaldag per student

### US_06 (3 SP) - Product Informatie
- Voorbeeldproducten zichtbaar in pakket
- Duidelijke disclaimer: geen garantie voor exacte inhoud
- Aantrekkelijke weergave

### US_07 (1 SP) - Dubbele Reserveringen Voorkomen
- First-come-first-served principe
- Klantvriendelijke foutmelding bij dubbele reservering

### EXTRA KEUZE (implementeer 1 van 3):

**US_08 (2 SP) - Filteren**
- Filter op locatie (studiestad als default)
- Filter op type maaltijd

**US_09 (1 SP) - Warme Maaltijd Restrictie**
- Alleen warme maaltijd pakketten op locaties die dit aanbieden
- Validatie bij pakket aanmaken

**US_10 (2 SP) - No-Show Management**
- No-shows registreren door kantinemedewerker
- Na 2+ no-shows: blokkering nieuwe reserveringen

## 🏗️ TECHNISCHE ARCHITECTUUR

### Onion Architecture - Project Structuur
```
AvansMaaltijdreservering/
├── AvansMaaltijdreservering.Core.Domain/          # Entities, Enums, Interfaces
├── AvansMaaltijdreservering.Core.DomainService/   # Business Logic Services  
├── AvansMaaltijdreservering.Infrastructure/       # Data Access, Repositories
├── AvansMaaltijdreservering.WebApp/              # ASP.NET MVC Core 8
├── AvansMaaltijdreservering.API/                 # RESTful API + GraphQL
├── AvansMaaltijdreservering.Core.Domain.Tests/   # Unit Tests
├── AvansMaaltijdreservering.Infrastructure.Tests/ # Infrastructure Tests
└── AvansMaaltijdreservering.sln
```

### Dependency Flow (Onion Architecture)
- **Dependencies wijzen ALTIJD naar binnen**
- Core.Domain: Geen dependencies
- Core.DomainService: → Core.Domain
- Infrastructure: → Core.Domain + Core.DomainService  
- WebApp/API: → Alle andere projecten

### Technische Requirements
- **ASP.NET Core 8** 
- **Entity Framework Code First** met migrations
- **Microsoft Identity** voor authenticatie (aparte database!)
- **Dependency Injection** container
- **Repository Pattern** met interfaces
- **Strongly Typed Views**
- **Lambda expressions** voor data filtering
- **Nullable Reference Types** enabled
- **Geen warnings/errors** tijdens build

## 🗄️ DATABASE ARCHITECTUUR

### Twee Databases (VERPLICHT!)
1. **Hoofddatabase:** Maaltijdreservering data
2. **Identity Database:** Gebruikers, rollen, authenticatie

### Belangrijke Relaties
- Student ↔ Pakket (reserveringen)
- Pakket ↔ Product (veel-op-veel - VERPLICHT!)
- Kantinemedewerker ↔ Kantine
- **Minimaal 1 veel-op-veel relatie vereist**

### Connection Strings
- **NOOIT** credentials in appsettings.json
- Gebruik Azure Configuration of Environment Variables

## 🔧 API REQUIREMENTS

### RESTful API (RMM Level 2)
- **Richardson Maturity Model Level 2**
- Standard HTTP verbs (GET, POST, PUT, DELETE)
- Resource-based URLs
- Proper HTTP status codes
- **Swagger documentatie**

### GraphQL Endpoint
- Losse endpoint naast REST API
- Voor mobile app (AvansOne team)
- Query capabilities voor pakketten + producten

### API Constraints Checklist
- ✅ Client/server architecture
- ✅ Stateless communication  
- ✅ Resources with multiple representations
- ✅ Standard operations

## 🧪 TESTING STRATEGY

### Unit Tests (VERPLICHT)
- **Business rules** uit acceptatiecriteria testen
- **Happy flow** + **error scenarios**  
- **Mocking** voor repositories
- **GEEN** simpele getters/setters testen
- **Per user story** tests uitwerken

### End-to-End Tests
- **Postman collecties** voor API endpoints
- **Automatisch uitvoerbaar** zonder manual token copy/paste
- Test alle CRUD operaties

## 🚀 CI/CD PIPELINE

### Development Pipeline
- **Automatic build** bij code push
- **Unit tests** automatisch uitvoeren  
- **Automatic deployment** naar Azure
- **Database migrations** via pipeline (NIET in code!)

### Azure Deployment
- Web App + API gedeployed
- Beide databases op Azure
- Connection strings via Azure Configuration

## 📋 VALIDATION RULES

### Student Validatie
- Geboortedatum niet in toekomst
- Minimaal 16 jaar bij aanmelden
- Uniek studentnummer + email
- Telefoonnummer formaat

### Pakket Validatie  
- Naam niet leeg
- Ophaaltijd in de toekomst
- Max 2 dagen vooruit plannen
- Prijs > 0
- Locatie validatie tegen kantinemedewerker locatie

### Product Validatie
- Naam niet leeg
- Foto vereist
- Alcohol flag correct

## 🎨 UX/UI REQUIREMENTS

### Design Principes
- **Gebruiksvriendelijk** interface
- **Consistent** design
- **Responsive** voor verschillende devices
- **Accessibility** compliance
- **Student-focused** design (denk aan target audience)

### Thema Keuze
- Specifieke studentgroepen targeten
- Deeltijd vs voltijd studenten
- Avondmaaltijden voor deeltijdstudenten
- **Kleur geven aan generieke opdracht**

## ⚠️ CRITICAL SUCCESS FACTORS

### Code Quality
- **Coding guidelines** naleven
- **Geen uitgecommentarieerde code**
- **Consistente naamgeving** (geen Class1.cs, Project1)
- **Netjes uitgelijnd**
- **EditorConfig** gebruiken

### Architecture Compliance
- **Onion Architecture** correct toepassen
- **Dependency injection** overal
- **Repository pattern** met interfaces
- **Separation of concerns**

### Oplevering Requirements
- **Gedeployde applicatie** URL
- **Test accounts** (kantinemedewerker + studenten)
- **Demo video** van alle functionaliteit
- **Postman collectie** werkend
- **Swagger documentatie** compleet
- **UML diagrammen** (package, class, component, deployment)

## 🚨 FAIL CONDITIONS (AUTO NV!)

- ❌ Rommelige oplevering (warnings, unused files)
- ❌ Connection strings met credentials in appsettings.json  
- ❌ Geen working deployed application
- ❌ Missing required artifacts
- ❌ EF migrations in program code
- ❌ Solution > 100MB (clean before zip!)

## 📊 PROJECT STATUS

### ✅ COMPLETED
- Solution structure volgens Onion Architecture
- All projects created with correct dependencies
- Basic project setup done

### 🔄 NEXT STEPS
1. Implement Domain Entities (Student, Pakket, Product, etc.)
2. Create Repository Interfaces  
3. Setup Entity Framework DbContexts (2 databases!)
4. Implement Domain Services
5. Create Controllers & Views
6. Unit Tests per User Story
7. API endpoints (REST + GraphQL)
8. CI/CD Pipeline setup
9. Azure deployment

---

## 📋 DEVELOPMENT HISTORY

### ✅ COMPLETED - 2025-07-01 - Core Domain Implementation
- **Domain Entities**: All entities implemented with English naming
  - Student (with age validation, no-show tracking)
  - Package (with business rules, alcohol detection) 
  - Product (with many-to-many to Package)
  - Canteen (with warm meal capability)
  - CanteenEmployee (with authorization logic)
- **Enums**: City, MealType, CanteenLocation
- **Repository Interfaces**: Full CRUD interfaces for all entities
- **DbContexts**: ApplicationDbContext (main data) + IdentityDbContext (auth)
- **Domain Services**: PackageService with all business rules implemented
- **Business Rules Implemented**:
  - 18+ automatic marking for alcohol products
  - Max 2 days advance planning for packages
  - Employee can only modify own canteen packages
  - No modification of reserved packages
  - Warm meal validation for canteen capabilities
  - Student age validation (min 16 years)
  - No-show tracking and blocking (2+ no-shows)
  - Max 1 package per pickup date per student

### ✅ COMPLETED - 2025-07-01 - Phase 2: Infrastructure & Frontend
- **Repository Implementations**: All repositories implemented with EF Core
  - StudentRepository, PackageRepository, ProductRepository
  - CanteenRepository, CanteenEmployeeRepository
  - Full CRUD operations with proper Include statements
- **Dependency Injection**: Complete DI setup in Program.cs
  - Two separate DbContexts registered (Application + Identity)
  - All repository and service interfaces registered
  - Microsoft Identity configured with custom options
- **Entity Framework Migrations**: Database setup completed
  - ApplicationDbContext with all entities and relationships
  - ApplicationIdentityDbContext for authentication
  - Canteen seed data included
  - Many-to-many Package-Product relationship configured
- **Modern Frontend Design**: Professional UI/UX implementation
  - Inter font family + Bootstrap 5 + Bootstrap Icons
  - Glass-morphism design with gradients and animations
  - Responsive mobile-first layout
  - "Avans Meal Rescue" branding with environmental theme
- **Controllers & Views**: Complete MVC implementation
  - PackageController: Available packages, CRUD for employees
  - StudentController: Dashboard, browsing, reservations
  - Beautiful views with filtering, stats cards, and user feedback
- **User Stories Implementation**:
  - US_01: Student dashboard with available packages & reservations
  - US_02: Canteen employee package management
  - US_03: Package CRUD with business rule validation
  - US_04: 18+ restrictions with visual indicators
  - US_05: Reservation system with comprehensive validation
  - US_06: Product information with "no guarantee" disclaimers
  - US_07: First-come-first-served with real-time status
  - US_08: Filtering by city (default: study city) and meal type

### 🔄 NEXT STEPS - Phase 3
1. **Authentication & Authorization** setup with roles
2. **Unit Tests** for business rules and validation
3. **API Implementation** (REST + GraphQL endpoints)
4. **Additional User Stories** (US_09 or US_10)
5. **Error Handling & Logging** implementation
6. **Performance Optimization** and caching
7. **Deployment Preparation** for Azure

### 🔍 REFLECTION - Phase 2 Architecture Review

After reflecting on what I made this phase, I can see both the strengths and critical gaps in the current implementation:

**✅ Strengths Achieved:**
- Clean Onion Architecture with proper dependency flow maintained throughout
- All 8 User Stories implemented with solid business rule validation
- Modern, responsive frontend with professional "Avans Meal Rescue" branding
- Two-database setup working correctly with seed data and relationships
- Comprehensive domain services with proper error handling

**❌ Critical Gaps Identified:**
1. **Authentication Placeholders**: Currently using `int currentStudentId = 1;` instead of real user claims
2. **Missing API Layer**: RESTful API (RMM Level 2) and GraphQL endpoints required but not implemented
3. **Zero Unit Tests**: Major gap for business rule validation and repository testing
4. **Race Conditions**: Reservation logic not thread-safe - could allow double-bookings
5. **Missing Validation Attributes**: Models lack proper `[Required]`, `[Range]` etc. for robust validation
6. **No Swagger Documentation**: Required for API but not implemented

**📊 Requirements Completion Assessment:**
- Architecture & Domain: 95% ✅
- Business Rules: 90% ✅  
- Frontend/UX: 95% ✅
- Database: 100% ✅
- API Layer: 0% ❌
- Testing: 0% ❌
- Authentication: 30% (setup only) ⚠️

**Verdict:** Strong foundation with 80% of requirements met. The architecture demonstrates solid understanding of domain design, but critical gaps in API, testing, and authentication need addressing for production readiness.

### 🎯 FOCUS AREAS
- **Business Rules Implementation** (alle acceptatiecriteria)
- **Data Validation** (age checks, reservering limits)
- **Security** (18+ restrictions, user authorization)
- **User Experience** (filtering, clear navigation)
- **Testing Coverage** (business logic focus)

## 💡 DEVELOPMENT TIPS

1. **Start met Domain Entities** - dit is je fundament
2. **Implementeer per User Story** - zo blijf je gefocused  
3. **Test Driven** - schrijf tests parallel aan development
4. **Clean Code** - refactor regelmatig, houd het netjes
5. **Git Workflow** - gebruik branches, commit vaak
6. **Documentation** - houd UML diagrammen up-to-date

Dit systeem simuleert een real-world food waste reduction platform met complexe business rules, multi-user scenarios, en enterprise-level technical requirements.