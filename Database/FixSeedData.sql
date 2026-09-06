-- ============================================================
-- Fix: Add Halls, Seats, Shows, and ShowSeats
-- Run this in SSMS after the movies are already inserted
-- ============================================================

USE CinemaHallDB;
GO

-- ============================================================
-- HALLS (3 halls)
-- ============================================================
INSERT INTO Halls (HallName, TotalSeats) VALUES
('Hall A - Gold',     80),
('Hall B - Silver',   48),
('Hall C - Platinum', 60);

-- ============================================================
-- SEATS
-- ============================================================
-- Hall 1: 8 rows (A-H) x 10 seats = 80
INSERT INTO Seats (HallID, SeatRow, SeatNo, SeatType)
SELECT 1, R.Letter, N.Num,
       CASE WHEN R.Letter IN ('A','B') THEN 'Premium' ELSE 'Regular' END
FROM (VALUES ('A'),('B'),('C'),('D'),('E'),('F'),('G'),('H')) AS R(Letter)
CROSS JOIN (VALUES (1),(2),(3),(4),(5),(6),(7),(8),(9),(10)) AS N(Num);

-- Hall 2: 6 rows (A-F) x 8 seats = 48
INSERT INTO Seats (HallID, SeatRow, SeatNo, SeatType)
SELECT 2, R.Letter, N.Num, 'Regular'
FROM (VALUES ('A'),('B'),('C'),('D'),('E'),('F')) AS R(Letter)
CROSS JOIN (VALUES (1),(2),(3),(4),(5),(6),(7),(8)) AS N(Num);

-- Hall 3: 6 rows (A-F) x 10 seats = 60
INSERT INTO Seats (HallID, SeatRow, SeatNo, SeatType)
SELECT 3, R.Letter, N.Num,
       CASE WHEN R.Letter = 'A' THEN 'Premium' ELSE 'Regular' END
FROM (VALUES ('A'),('B'),('C'),('D'),('E'),('F')) AS R(Letter)
CROSS JOIN (VALUES (1),(2),(3),(4),(5),(6),(7),(8),(9),(10)) AS N(Num);

-- ============================================================
-- SHOWS (7 shows across next few days)
-- ============================================================
DECLARE @m1 INT = (SELECT TOP 1 MovieID FROM Movies WHERE Title = 'The Dark Knight');
DECLARE @m2 INT = (SELECT TOP 1 MovieID FROM Movies WHERE Title = 'Inception');
DECLARE @m3 INT = (SELECT TOP 1 MovieID FROM Movies WHERE Title = 'Parasite');
DECLARE @m4 INT = (SELECT TOP 1 MovieID FROM Movies WHERE Title = 'The Grand Budapest Hotel');
DECLARE @m5 INT = (SELECT TOP 1 MovieID FROM Movies WHERE Title = 'The Shawshank Redemption');
DECLARE @m6 INT = (SELECT TOP 1 MovieID FROM Movies WHERE Title = 'Get Out');

INSERT INTO Shows (MovieID, HallID, ShowDate, ShowTime, TicketPrice) VALUES
(@m1, 1, CAST(DATEADD(DAY, 1, GETDATE()) AS DATE), '14:00', 350.00),
(@m1, 1, CAST(DATEADD(DAY, 1, GETDATE()) AS DATE), '18:00', 400.00),
(@m2, 2, CAST(DATEADD(DAY, 1, GETDATE()) AS DATE), '15:00', 300.00),
(@m3, 3, CAST(DATEADD(DAY, 2, GETDATE()) AS DATE), '14:00', 350.00),
(@m4, 1, CAST(DATEADD(DAY, 2, GETDATE()) AS DATE), '20:00', 450.00),
(@m5, 2, CAST(DATEADD(DAY, 3, GETDATE()) AS DATE), '16:00', 300.00),
(@m6, 3, CAST(DATEADD(DAY, 3, GETDATE()) AS DATE), '19:00', 350.00);

-- ============================================================
-- SHOWSEATS (auto-generate)
-- ============================================================
INSERT INTO ShowSeats (ShowID, SeatID, Status)
SELECT s.ShowID, st.SeatID, 'Available'
FROM Shows s
INNER JOIN Seats st ON st.HallID = s.HallID;

-- ============================================================
-- FOOD ITEMS
-- ============================================================
INSERT INTO FoodItems (Name, Price, Category) VALUES
('Large Popcorn',      250.00, 'Snacks'),
('Small Popcorn',      150.00, 'Snacks'),
('Coca Cola',          100.00, 'Beverages'),
('Nachos with Cheese', 200.00, 'Snacks'),
('Mineral Water',       50.00, 'Beverages'),
('Hot Dog',            180.00, 'Meals');

PRINT 'Done! 3 Halls + 188 Seats + 7 Shows + ShowSeats + 6 Food Items added.';
GO
