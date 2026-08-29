# RaceDay – Event Management System
**PROG6212 Programming 2B – PoE Part 1**

## Description
RaceDay is a full-stack web-based event management system designed specifically for the South African road running, walking, and cycling community. The platform allows Event Organisers to create and manage events, categories, and participant results, while Participants can browse upcoming events, enrol in categories, track their personal performance history, and prepare for race day.

This repository contains **Part 1: System Planning and Database**.

## User Roles

### Organiser
- Create, edit, and delete events
- Manage event categories
- View all enrolments for their events
- Capture and publish participant results

### Participant
- Create an account and log in
- Browse available events
- Enrol in an event by selecting a category
- View personal enrolments
- Track personal race results and performance history

## Repository Structure

RaceDay/
├── README.md
├── docs/
│   ├── RaceDay_ERD.png                  # Entity Relationship Diagram
│   ├── RaceDay_API_Endpoint_Plan.md     # Full API Endpoint Plan
│   └── RaceDay_Database.sql             # SQL Database Script
└── .github/
└── workflows/
└── part1-ci.yml                 # GitHub Actions CI workflow


## How to Run the SQL Script

1. Open **SQL Server Management Studio (SSMS)**.
2. Connect to your local SQL Server instance.
3. Open the file `docs/RaceDay_Database.sql`.
4. Execute the entire script (press F5).
5. Refresh Object Explorer — you should see the `RaceDayDB` database with all tables and seed data.

## CI/CD
A GitHub Actions workflow is included that validates the repository structure on every push.

**Successful CI Build Screenshot:**

![CI Green Build](docs/ci-screenshot.png)

*(Replace this with your actual screenshot later)*

## Video Presentation
Unlisted YouTube video link:  
**[Insert your unlisted YouTube video link here]**

In the video I explain:
- ERD design decisions
- API Endpoint Plan choices
- Running the SQL script live in SSMS

## Notes
- The ERD, API Endpoint Plan, and SQL script are fully consistent with each other.
- No API or MVC application code is included in Part 1 (as required by the brief).
- Minimum of 20 meaningful commits will be made for this part.