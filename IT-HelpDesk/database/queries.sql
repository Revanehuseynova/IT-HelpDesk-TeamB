/* IT Help Desk - useful SQL queries                      (Database Developer) */
USE ITHelpDeskDb;
GO

-- 1. Total number of tickets and counts per status
SELECT * FROM dbo.vw_TicketStatistics;

-- 2. Tickets per status (readable names)
SELECT CASE [Status] WHEN 0 THEN N'New' WHEN 1 THEN N'In Progress'
                     WHEN 2 THEN N'Resolved' WHEN 3 THEN N'Closed' END AS StatusName,
       COUNT(*) AS Total
FROM dbo.Tickets GROUP BY [Status] ORDER BY [Status];

-- 3. Tickets per category
SELECT c.Name AS Category, COUNT(t.Id) AS Total
FROM dbo.Categories c LEFT JOIN dbo.Tickets t ON t.CategoryId = c.Id
GROUP BY c.Name ORDER BY Total DESC;

-- 4. Full ticket list with creator, category and assignee
SELECT t.Id, t.Title, c.Name AS Category, t.Priority, t.[Status],
       u.FullName AS CreatedBy, a.FullName AS AssignedTo, t.CreatedAt
FROM dbo.Tickets t
JOIN dbo.Categories c ON c.Id = t.CategoryId
JOIN dbo.Users u      ON u.Id = t.CreatedById
LEFT JOIN dbo.Users a ON a.Id = t.AssignedToId
ORDER BY t.CreatedAt DESC;

-- 5. Filter by category and status (example: Hardware + In Progress)
SELECT t.Id, t.Title FROM dbo.Tickets t
WHERE t.CategoryId = 1 AND t.[Status] = 1;

-- 6. Unassigned tickets (waiting for an IT specialist)
SELECT Id, Title, Priority, CreatedAt FROM dbo.Tickets
WHERE AssignedToId IS NULL AND [Status] = 0 ORDER BY Priority DESC, CreatedAt;

-- 7. Workload of each IT specialist
SELECT u.FullName, COUNT(t.Id) AS ActiveTickets
FROM dbo.Users u LEFT JOIN dbo.Tickets t ON t.AssignedToId = u.Id AND t.[Status] IN (1)
WHERE u.[Role] = N'ITSpecialist' GROUP BY u.FullName;
