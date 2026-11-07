/* =====================================================================
   IT Help Desk - database.sql                          (Database Developer)
   Target : Microsoft SQL Server 2019+ / LocalDB / Express / Docker
   Run    : SSMS, Azure Data Studio or  sqlcmd -S "(localdb)\MSSQLLocalDB" -i database.sql
   WARNING: Script is RE-RUNNABLE for development: it DROPS and recreates
            the tables (all data is lost). Never run on production data.
   Enum values used by the application:
     Status   : 0 = New, 1 = In Progress, 2 = Resolved, 3 = Closed
     Priority : 0 = Low, 1 = Medium, 2 = High, 3 = Critical
   ===================================================================== */

IF DB_ID(N'ITHelpDeskDb') IS NULL
    CREATE DATABASE ITHelpDeskDb;
GO

USE ITHelpDeskDb;
GO

/* ---------- Drop (dependency order) ---------- */
IF OBJECT_ID(N'dbo.vw_TicketStatistics', N'V') IS NOT NULL DROP VIEW dbo.vw_TicketStatistics;
IF OBJECT_ID(N'dbo.Tickets',    N'U') IS NOT NULL DROP TABLE dbo.Tickets;
IF OBJECT_ID(N'dbo.Categories', N'U') IS NOT NULL DROP TABLE dbo.Categories;
IF OBJECT_ID(N'dbo.Users',      N'U') IS NOT NULL DROP TABLE dbo.Users;
GO

/* ---------- Users ---------- */
CREATE TABLE dbo.Users (
    Id           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
    FullName     NVARCHAR(100) NOT NULL,
    Email        NVARCHAR(150) NOT NULL,
    PasswordHash NVARCHAR(500) NOT NULL,   -- PBKDF2: iterations.salt.hash (never plain text)
    [Role]       NVARCHAR(30)  NOT NULL,
    CreatedAt    DATETIME2     NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT SYSUTCDATETIME(),
    CONSTRAINT UQ_Users_Email UNIQUE (Email),
    CONSTRAINT CK_Users_Role CHECK ([Role] IN (N'Student', N'Employee', N'ITSpecialist'))
);
GO

/* ---------- Categories ---------- */
CREATE TABLE dbo.Categories (
    Id   INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Categories PRIMARY KEY,
    Name NVARCHAR(80) NOT NULL,
    CONSTRAINT UQ_Categories_Name UNIQUE (Name)
);
GO

/* ---------- Tickets ---------- */
CREATE TABLE dbo.Tickets (
    Id           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Tickets PRIMARY KEY,
    Title        NVARCHAR(150)  NOT NULL,
    [Description] NVARCHAR(2000) NOT NULL,
    CategoryId   INT NOT NULL,
    Priority     INT NOT NULL CONSTRAINT DF_Tickets_Priority DEFAULT 1,
    [Status]     INT NOT NULL CONSTRAINT DF_Tickets_Status   DEFAULT 0,
    CreatedById  INT NOT NULL,
    AssignedToId INT NULL,
    CreatedAt    DATETIME2 NOT NULL CONSTRAINT DF_Tickets_CreatedAt DEFAULT SYSUTCDATETIME(),
    UpdatedAt    DATETIME2 NOT NULL CONSTRAINT DF_Tickets_UpdatedAt DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Tickets_Categories FOREIGN KEY (CategoryId)   REFERENCES dbo.Categories(Id),
    CONSTRAINT FK_Tickets_CreatedBy  FOREIGN KEY (CreatedById)  REFERENCES dbo.Users(Id),
    CONSTRAINT FK_Tickets_AssignedTo FOREIGN KEY (AssignedToId) REFERENCES dbo.Users(Id),
    CONSTRAINT CK_Tickets_Status   CHECK ([Status] BETWEEN 0 AND 3),
    CONSTRAINT CK_Tickets_Priority CHECK (Priority BETWEEN 0 AND 3)
);
GO

CREATE INDEX IX_Tickets_Status      ON dbo.Tickets([Status]);
CREATE INDEX IX_Tickets_CategoryId  ON dbo.Tickets(CategoryId);
CREATE INDEX IX_Tickets_CreatedById ON dbo.Tickets(CreatedById);
CREATE INDEX IX_Tickets_AssignedToId ON dbo.Tickets(AssignedToId);
GO

/* ---------- Statistics view (total + per status) ---------- */
CREATE VIEW dbo.vw_TicketStatistics AS
SELECT
    COUNT(*)                                            AS TotalTickets,
    SUM(CASE WHEN [Status] = 0 THEN 1 ELSE 0 END)       AS NewTickets,
    SUM(CASE WHEN [Status] = 1 THEN 1 ELSE 0 END)       AS InProgressTickets,
    SUM(CASE WHEN [Status] = 2 THEN 1 ELSE 0 END)       AS ResolvedTickets,
    SUM(CASE WHEN [Status] = 3 THEN 1 ELSE 0 END)       AS ClosedTickets
FROM dbo.Tickets;
GO

/* ---------- Sample data ---------- */
/* Development passwords (see README):
     it.specialist@university.edu  /  IT@12345
     student@university.edu        /  Student@123
     employee@university.edu       /  Employee@123                       */
INSERT INTO dbo.Users (FullName, Email, PasswordHash, [Role]) VALUES
    (N'Aysel Mammadova', N'it.specialist@university.edu', N'100000.ve8Is0fyKXmiuq9xxJx2bw==.SBn947nN3lHggyapYRk1A1GmqyjS2XXFb/Sg3hPivM0=', N'ITSpecialist'),
    (N'Murad Aliyev', N'student@university.edu', N'100000.rNq7o+I9e0C41hT8lnwNxg==.2mAmd0yspZ5uM74TyGptdb/V7BASTFNo3LepCAw6Ml8=', N'Student'),
    (N'Leyla Hasanova', N'employee@university.edu', N'100000.liYvQ004iUUJ9PAf2bzYZg==.SjUeGa1/hky4uaWlC7AdpO46BIljBluJ86GBPeCkR2Q=', N'Employee');

INSERT INTO dbo.Categories (Name) VALUES
    (N'Hardware'), (N'Software'), (N'Network / Internet'),
    (N'Account & Password'), (N'Email'), (N'Printer'), (N'Other');

-- Users: 1 = IT specialist, 2 = student, 3 = employee
-- Categories: 1 Hardware, 2 Software, 3 Network, 4 Account, 5 Email, 6 Printer, 7 Other
INSERT INTO dbo.Tickets (Title, [Description], CategoryId, Priority, [Status], CreatedById, AssignedToId, CreatedAt, UpdatedAt) VALUES
 (N'Cannot connect to campus Wi-Fi', N'My laptop does not connect to the campus Wi-Fi in building B since this morning.', 3, 2, 0, 2, NULL, DATEADD(HOUR,-30,SYSUTCDATETIME()), DATEADD(HOUR,-30,SYSUTCDATETIME())),
 (N'Forgot student portal password', N'I cannot sign in to the student portal and the reset e-mail never arrives.', 4, 1, 1, 2, 1, DATEADD(DAY,-2,SYSUTCDATETIME()), DATEADD(DAY,-1,SYSUTCDATETIME())),
 (N'Printer on 2nd floor is jammed', N'The printer in room 214 shows a paper jam error and does not print.', 6, 1, 2, 3, 1, DATEADD(DAY,-4,SYSUTCDATETIME()), DATEADD(DAY,-3,SYSUTCDATETIME())),
 (N'Install Visual Studio in lab 3', N'Please install Visual Studio 2022 on all computers in lab 3 for the C# course.', 2, 1, 3, 3, 1, DATEADD(DAY,-10,SYSUTCDATETIME()), DATEADD(DAY,-7,SYSUTCDATETIME())),
 (N'Monitor flickers', N'The monitor on my desk flickers every few minutes and sometimes turns black.', 1, 2, 1, 3, 1, DATEADD(DAY,-1,SYSUTCDATETIME()), DATEADD(HOUR,-5,SYSUTCDATETIME())),
 (N'University e-mail is full', N'My mailbox quota is exceeded, I cannot receive new messages.', 5, 3, 0, 2, NULL, DATEADD(HOUR,-3,SYSUTCDATETIME()), DATEADD(HOUR,-3,SYSUTCDATETIME())),
 (N'Projector does not work in room 101', N'The projector shows no signal when an HDMI cable is connected.', 1, 3, 0, 3, NULL, DATEADD(HOUR,-8,SYSUTCDATETIME()), DATEADD(HOUR,-8,SYSUTCDATETIME())),
 (N'Request for VPN access', N'I need VPN access to reach the library databases from home.', 3, 0, 2, 2, 1, DATEADD(DAY,-6,SYSUTCDATETIME()), DATEADD(DAY,-5,SYSUTCDATETIME()));
GO
