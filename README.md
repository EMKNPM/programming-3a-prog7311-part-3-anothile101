# Global Logistics Management System (GLMS)

# Project Overview

GLMS is an enterprise-grade logistics management platform for TechMove Logistics. The system manages international freight contracts, client relationships, service requests, and financial integrations.

# Technologies Used: 
- ASP.NET Core MVC (.NET 9.0)
- ASP.NET Core Web API (.NET 9.0)
- Entity Framework Core 
- SQL Server / LocalDB
- JWT Authentication
- Docker & Docker Compose
- xUnit for Testing

---

# Part 2: Monolithic MVC Application

# Features Implemented

# 1. Database Design (Entity Framework Core)

**Entities:** 

 **Client**:  Id, Name, ContactDetails, Region
 **Contract**:  Id, ClientId, StartDate, EndDate, Status (Draft/Active/Expired/OnHold), ServiceLevel (Gold/Silver/Bronze), SignedAgreementPath |
 **ServiceRequest**: Id, ContractId, Description, CostUsd, CostZar, ExchangeRateUsed, Status (Pending/InProgress/Completed/Cancelled), CreatedAt |

---
#### 2. Design Patterns Implemented

 **Repository**  
 - Data access abstraction
 - 
 **Factory**
 - Contract creation with status logic (Gold = Active, Silver/Bronze = Draft)
 
 **Observer** 
 - Contract status change notifications |

   
---
# 3. Workflow & Business Logic

- **Date Validation:** End date must be after start date
- **Status Workflow:** Service requests cannot be created for Expired or OnHold contracts
- **Search/Filter:** LINQ-based filtering by date range and status

# 4. File Handling

- Upload PDF "Signed Agreement" for each contract
- Files saved to server with GUID naming to prevent overwrites
- Validation: Only PDF files allowed, max 10MB size limit

---
# 5. External API Integration

- Consumes ExchangeRate-API for USD to ZAR conversion
- Real-time exchange rates applied to service request costs

---
# 6. Unit Testing (xUnit)

**Test Categories:**
- ContractDateValidationTests
- ContractFactoryTests
- CurrencyCalculationTests
- FileValidationTests
- WorkflowValidationTests

**Total Tests:** 69 passing

---

# Part 3: Service-Oriented Architecture with Docker

# Architecture Overview

# Web API (Backend)

**Endpoints:**

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/Auth/login` | JWT Authentication |
| GET | `/api/Clients` | Get all clients |
| GET | `/api/Clients/{id}` | Get client by ID |
| POST | `/api/Clients` | Create client |
| GET | `/api/Contracts` | Get all contracts |
| GET | `/api/Contracts/{id}` | Get contract by ID |
| GET | `/api/Contracts/filter` | Filter by date/status |
| POST | `/api/Contracts` | Create contract (Gold = Active) |
| PATCH | `/api/Contracts/{id}/status` | Update contract status |
| GET | `/api/ServiceRequests` | Get service requests |
| POST | `/api/ServiceRequests` | Create service request (with currency conversion) |
| GET | `/api/Currency/rate` | Get USD/ZAR exchange rate |
| POST | `/api/Currency/convert` | Convert USD to ZAR |

---
**Authentication:** JWT Bearer tokens required for all endpoints except login.

---
# Repository & Service Layer (API Backend)

---
**Repositories**  
- `IContractRepository`, `ContractRepository`, `IClientRepository`, `ClientRepository` 

  ---
**Services**
- `IContractService`, `ContractService`, `IClientService`, `ClientService` 

---
# MVC Frontend (Refactored)

The MVC application no longer connects directly to the database. Instead:
- Uses `HttpClient` to call the Web API
- Stores JWT token in session
- All CRUD operations go through the API

---
# Docker Configuration

**Three containers:**

| Container | Image | Port |
|-----------|-------|------|
| sql-server-db | mcr.microsoft.com/mssql/server:2022-latest | 1433 |
| glms-backend-api | Custom (PracticeAssignment.API) | 7001 |
| glms-frontend-web | Custom (Practice assignment) | 5000 |


---
# Youtube Link: 
https://youtu.be/SkbgyPz5wYw 
