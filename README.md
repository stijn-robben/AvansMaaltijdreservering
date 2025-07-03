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


Dit systeem simuleert een real-world food waste reduction platform met complexe business rules, multi-user scenarios, en enterprise-level technical requirements.
