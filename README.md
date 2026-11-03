# RaceDay – Full-Stack Event Management System

**Module:** PROG6212 – Programming 2B  
**Student Name:** Monique Viljoen  
**Student Number:** ST10465636

## Project Description

RaceDay is a full-stack event management system designed for the South African road running, walking, and cycling community.  
The system allows Event Organisers to create and manage events, categories, and results, while Participants can browse events, enrol, and track their personal performance.

This repository contains all three parts of the Portfolio of Evidence.

---

## User Roles

### Organiser
- Register and log in
- Create, update, and delete events
- Manage categories for events
- View enrolments for their events
- Capture race results (finish times and positions)

### Participant
- Register and log in
- Browse available events and categories
- Enrol in events
- View personal enrolments and results
- Update their own profile

---

## Part 1 – System Planning and Database

- Entity Relationship Diagram (ERD)
- API Endpoint Plan
- SQL Database Script

All planning documents are available in the `/docs` folder.

**Part 1 Video:** https://youtu.be/qv_kAdhozjU

---

## Part 2 – RESTful API Development

### Technologies Used
- ASP.NET Core Web API (.NET 8)
- Entity Framework Core (Code-First)
- SQL Server
- Session-based Authentication
- BCrypt Password Hashing
- Swagger / OpenAPI
- xUnit Unit Tests
- GitHub Actions CI/CD

### How to Run the API

1. Open the solution (`RaceDay.Api.sln`) in Visual Studio 2022.
2. Update the connection string in `appsettings.json` if needed.
3. Run the following commands in the Package Manager Console:
4. Press `F5` to run the API.
5. Swagger will open at: `https://localhost:xxxx/swagger`

### Running the Unit Tests

1. Open **Test Explorer** in Visual Studio.
2. Click **Run All**.
3. All tests should pass.

### API Endpoints Overview
- Authentication (`/api/auth`)
- Profile (`/api/profile`)
- Events (`/api/events`)
- Categories (`/api/categories`)
- Enrolments (`/api/enrolments`)
- Results (`/api/results`)

### CI/CD
A GitHub Actions workflow is configured to restore, build, and run all unit tests on every push.

**CI Build Screenshot:**  
![CI Build](./docs/part2-ci-screenshot.png)

**Part 2 Video:** https://youtu.be/jQnxK9J5PcQ

## Part 3 – MVC Front End + Azure Blob Storage + Docker
### Technologies Used
- ASP.NET Core MVC (.NET 8)
- HttpClient (consuming Part 2 API)
- Session-based Authentication
- Azure Blob Storage (or Azurite for local development)
- Docker & Docker Compose
- GitHub Actions CI/CD

### How to Run with Docker (Recommended)
1. Make sure Docker Desktop is running.
2. In the root of the repository, run:
   ```bash
   docker-compose up -d
   ```
3. Open the web application:  
   **http://localhost:8081**
4. API (Swagger):  
   **http://localhost:8080/swagger**

### How to Run without Docker
1. Start the API project (`RaceDay.Api`).
2. Start the MVC project (`RaceDay.Web`).
3. Update the `BaseAddress` in `RaceDay.Web/Program.cs` if your API port is different.

### Features Demonstrated
- Role-based navigation and pages
- Full event → category → enrolment → result flow
- Profile and event banner image uploads (Azure Blob / Azurite)
- Dockerised environment

### CI/CD
The Part 3 GitHub Actions workflow builds the entire solution (including the MVC project) and runs the unit tests.

**CI Build Screenshot:**  
![CI Build](./docs/Part3 ci-green.png)

**Part 3 Video:** https://youtu.be/YOUR_PART3_VIDEO_ID

---

## Repository Structure
```bash
RaceDay/                                      ← Root of the repository
│
├── docs/                                     ← Part 1 planning documents + CI screenshots
│   ├── RaceDay_ERD.pdf
│   ├── RaceDay_API_Endpoint_Plan.md
│   ├── RaceDay_Database.sql
│   ├── part2-ci-screenshot.png
│   └── ci-green.png                          ← Part 3 green CI screenshot
│
├── RaceDay.Api/                              ← ASP.NET Core Web API (Part 2)
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── ProfileController.cs
│   │   ├── EventsController.cs
│   │   ├── CategoriesController.cs
│   │   ├── EnrolmentsController.cs
│   │   └── ResultsController.cs
│   ├── Data/
│   │   └── RaceDayDbContext.cs
│   ├── Models/
│   │   ├── AppUser.cs
│   │   ├── Event.cs
│   │   ├── Category.cs
│   │   ├── Enrolment.cs
│   │   └── Result.cs
│   ├── Migrations/
│   ├── appsettings.json
│   ├── Program.cs
│   └── RaceDay.Api.csproj
│
├── RaceDay.Web/                              ← ASP.NET Core MVC (Part 3)
│   ├── Controllers/
│   │   ├── AccountController.cs
│   │   ├── HomeController.cs
│   │   ├── OrganiserController.cs
│   │   ├── EventsController.cs
│   │   ├── CategoriesController.cs
│   │   ├── EnrolmentsController.cs
│   │   ├── ResultsController.cs
│   │   └── ProfileController.cs
│   ├── Services/
│   │   └── ApiService.cs
│   ├── Views/
│   │   ├── Account/
│   │   ├── Organiser/
│   │   ├── Events/
│   │   ├── Categories/
│   │   ├── Enrolments/
│   │   ├── Results/
│   │   ├── Profile/
│   │   └── Shared/
│   ├── wwwroot/
│   ├── Program.cs
│   └── RaceDay.Web.csproj
│
├── RaceDay.Contracts/                        ← Shared DTOs (Part 2 & 3)
│   ├── Dtos.cs
│   └── RaceDay.Contracts.csproj
│
├── RaceDay.Api.Tests/                        ← Unit Tests (Part 2)
│   ├── AuthAndRoleTests.cs
│   └── RaceDay.Api.Tests.csproj
│
├── .github/
│   └── workflows/
│       ├── part1-ci.yml                      ← Part 1 Structure Validation
│       ├── part2-ci.yml                      ← Part 2 CI workflow
│       └── part3-ci.yml                      ← Part 3 CI workflow
│
├── docker-compose.yml                        ← Docker setup (Part 3)
├── RaceDay.Api.sln                           ← Solution file
└── README.md
```

---

*PROG6212 – Programming 2B Portfolio of Evidence*


