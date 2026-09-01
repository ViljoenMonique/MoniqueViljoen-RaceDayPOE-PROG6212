-- =============================================
-- RaceDay Database Script
-- PROG6212 PoE Part 1
-- Must match the ERD exactly
-- =============================================

USE master;
GO

IF DB_ID('RaceDayDB') IS NOT NULL
BEGIN
    ALTER DATABASE RaceDayDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE RaceDayDB;
END
GO

CREATE DATABASE RaceDayDB;
GO

USE RaceDayDB;
GO

-- =====================
-- TABLES
-- =====================

CREATE TABLE [User] (
    UserID            INT IDENTITY(1,1) PRIMARY KEY,
    Email             NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash      NVARCHAR(255) NOT NULL,
    FirstName         NVARCHAR(100) NOT NULL,
    LastName          NVARCHAR(100) NOT NULL,
    Role              NVARCHAR(20)  NOT NULL CHECK (Role IN ('Organiser', 'Participant')),
    Phone             NVARCHAR(20)  NULL,
    ProfileImageUrl   NVARCHAR(500) NULL,
    DateCreated       DATETIME2     NOT NULL DEFAULT SYSDATETIME()
);
GO

CREATE TABLE Event (
    EventID           INT IDENTITY(1,1) PRIMARY KEY,
    OrganiserID       INT NOT NULL,
    EventName         NVARCHAR(200) NOT NULL,
    Description       NVARCHAR(MAX) NULL,
    EventDate         DATE NOT NULL,
    Location          NVARCHAR(200) NOT NULL,
    DistanceKm        DECIMAL(6,2) NULL,
    EventType         NVARCHAR(50) NOT NULL,
    MaxParticipants   INT NULL,
    Status            NVARCHAR(20) NOT NULL DEFAULT 'Open'
        CHECK (Status IN ('Open', 'Closed', 'Completed', 'Cancelled')),
    CONSTRAINT FK_Event_Organiser FOREIGN KEY (OrganiserID) 
        REFERENCES [User](UserID)
);
GO

CREATE TABLE Category (
    CategoryID          INT IDENTITY(1,1) PRIMARY KEY,
    EventID             INT NOT NULL,
    CategoryName        NVARCHAR(100) NOT NULL,
    DistanceKm          DECIMAL(6,2) NULL,
    AgeMin              INT NULL,
    AgeMax              INT NULL,
    GenderRestriction   NVARCHAR(20) NULL,
    EntryFee            DECIMAL(10,2) NOT NULL DEFAULT 0,
    CONSTRAINT FK_Category_Event FOREIGN KEY (EventID) 
        REFERENCES Event(EventID) ON DELETE CASCADE
);
GO

CREATE TABLE Enrolment (
    EnrolmentID     INT IDENTITY(1,1) PRIMARY KEY,
    ParticipantID   INT NOT NULL,
    EventID         INT NOT NULL,
    CategoryID      INT NOT NULL,
    EnrolmentDate   DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    Status          NVARCHAR(20) NOT NULL DEFAULT 'Confirmed'
        CHECK (Status IN ('Confirmed', 'Cancelled', 'Waitlisted')),
    CONSTRAINT FK_Enrolment_Participant FOREIGN KEY (ParticipantID) 
        REFERENCES [User](UserID),
    CONSTRAINT FK_Enrolment_Event FOREIGN KEY (EventID) 
        REFERENCES Event(EventID),
    CONSTRAINT FK_Enrolment_Category FOREIGN KEY (CategoryID) 
        REFERENCES Category(CategoryID),
    CONSTRAINT UQ_Participant_Event UNIQUE (ParticipantID, EventID)
);
GO

CREATE TABLE Result (
    ResultID      INT IDENTITY(1,1) PRIMARY KEY,
    EnrolmentID   INT NOT NULL UNIQUE,
    FinishTime    TIME(0) NULL,
    Position      INT NULL,
    Status        NVARCHAR(20) NOT NULL DEFAULT 'Finished'
        CHECK (Status IN ('Finished', 'DNF', 'DNS')),
    Notes         NVARCHAR(500) NULL,
    CONSTRAINT FK_Result_Enrolment FOREIGN KEY (EnrolmentID) 
        REFERENCES Enrolment(EnrolmentID)
);
GO

CREATE TABLE EventImage (
    ImageID     INT IDENTITY(1,1) PRIMARY KEY,
    EventID     INT NOT NULL,
    ImageUrl    NVARCHAR(500) NOT NULL,
    Caption     NVARCHAR(200) NULL,
    IsPrimary   BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_EventImage_Event FOREIGN KEY (EventID) 
        REFERENCES Event(EventID) ON DELETE CASCADE
);
GO

-- =====================
-- SEED DATA
-- =====================

-- 2 Organisers + 2 Participants
INSERT INTO [User] (Email, PasswordHash, FirstName, LastName, Role, Phone) VALUES
('thabo.mokoena@raceday.co.za', 'AQAAAAEAACcQAAAAEHash1', 'Thabo', 'Mokoena', 'Organiser', '0821112233'),
('lerato.dlamini@raceday.co.za', 'AQAAAAEAACcQAAAAEHash2', 'Lerato', 'Dlamini', 'Organiser', '0834445566'),
('sipho.nkosi@gmail.com', 'AQAAAAEAACcQAAAAEHash3', 'Sipho', 'Nkosi', 'Participant', '0712223344'),
('nomsa.khumalo@gmail.com', 'AQAAAAEAACcQAAAAEHash4', 'Nomsa', 'Khumalo', 'Participant', '0725556677');
GO

-- 3 Events
INSERT INTO Event (OrganiserID, EventName, Description, EventDate, Location, DistanceKm, EventType, MaxParticipants, Status) VALUES
(1, 'Soweto Marathon 2026', 'The iconic road race through the heart of Soweto', '2026-11-01', 'Soweto, Johannesburg', 42.20, 'Running', 15000, 'Open'),
(1, 'Cape Town Cycle Tour Fun Ride', 'Scenic cycle event around the Cape Peninsula', '2026-03-08', 'Cape Town', 42.00, 'Cycling', 5000, 'Open'),
(2, 'Durban Beachfront Walk', 'Family-friendly beach walk along the Golden Mile', '2026-05-15', 'Durban', 10.00, 'Walking', 2000, 'Open');
GO

-- Categories
INSERT INTO Category (EventID, CategoryName, DistanceKm, AgeMin, AgeMax, GenderRestriction, EntryFee) VALUES
(1, '42km Marathon', 42.20, 18, NULL, NULL, 450.00),
(1, '21km Half Marathon', 21.10, 16, NULL, NULL, 350.00),
(1, '10km Fun Run', 10.00, 12, NULL, NULL, 200.00),
(2, '42km Cycle', 42.00, 16, NULL, NULL, 380.00),
(2, '21km Cycle', 21.00, 14, NULL, NULL, 280.00),
(3, '10km Walk', 10.00, 10, NULL, NULL, 150.00),
(3, '5km Family Walk', 5.00, 5, NULL, NULL, 80.00);
GO

-- Enrolments
INSERT INTO Enrolment (ParticipantID, EventID, CategoryID, Status) VALUES
(3, 1, 1, 'Confirmed'),   -- Sipho in Soweto Marathon 42km
(3, 2, 4, 'Confirmed'),   -- Sipho in Cape Town Cycle 42km
(4, 1, 2, 'Confirmed'),   -- Nomsa in Soweto Half Marathon
(4, 3, 6, 'Confirmed');   -- Nomsa in Durban 10km Walk
GO

-- Results
INSERT INTO Result (EnrolmentID, FinishTime, Position, Status, Notes) VALUES
(1, '03:45:22', 156, 'Finished', 'Strong finish'),
(3, '01:52:10', 89, 'Finished', NULL);
GO

-- Event Images
INSERT INTO EventImage (EventID, ImageUrl, Caption, IsPrimary) VALUES
(1, 'https://raceday.blob.core.windows.net/events/soweto-main.jpg', 'Soweto Marathon Start', 1),
(2, 'https://raceday.blob.core.windows.net/events/cpt-cycle.jpg', 'Cape Town Cycle Tour', 1),
(3, 'https://raceday.blob.core.windows.net/events/durban-walk.jpg', 'Durban Beachfront', 1);
GO

PRINT 'RaceDayDB created and seeded successfully.';
