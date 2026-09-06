-- ============================================================================
-- Cinema Hall Management System — Master Database Setup Script
-- ============================================================================
-- This script completely sets up the database from scratch:
-- 1. Creates the database 'CinemaHallDB' if it doesn't already exist.
-- 2. Safely drops existing tables (in reverse foreign-key order).
-- 3. Creates all 11 tables with keys, foreign keys, check constraints & indexes.
-- 4. Seeds sample data:
--    - 3 Users (Admin, Staff, Customer with pre-hashed passwords)
--    - 3 Cinema Halls (Gold, Silver, Platinum)
--    - 188 Physical Seats (Rows A-H with Premium and Regular tiers)
--    - 12 Movies (2 per genre: Action, Sci-Fi, Thriller, Comedy, Drama, Horror)
--    - 7 Shows scheduled dynamically over the upcoming days
--    - ~400+ ShowSeats pre-generated and ready for booking
--    - 6 Concession Food Items (Popcorn, Nachos, Beverages, Meals)
--
-- HOW TO RUN:
-- 1. Open SQL Server Management Studio (SSMS).
-- 2. Connect to your SQL Server instance (e.g. localhost, ., or .\SQLEXPRESS).
-- 3. Open this file and click "Execute" (F5).
-- ============================================================================

-- Step 1: Create Database if it does not exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'CinemaHallDB')
BEGIN
    CREATE DATABASE CinemaHallDB;
    PRINT '✓ Database [CinemaHallDB] created.';
END
ELSE
BEGIN
    PRINT 'ℹ Database [CinemaHallDB] already exists.';
END
GO

USE CinemaHallDB;
GO

-- Step 2: Drop existing tables in reverse dependency order
PRINT 'Dropping old tables if they exist...';

DROP TABLE IF EXISTS FoodOrderItems;
DROP TABLE IF EXISTS FoodOrders;
DROP TABLE IF EXISTS FoodItems;
DROP TABLE IF EXISTS Payments;
DROP TABLE IF EXISTS BookingSeats;
DROP TABLE IF EXISTS Bookings;
DROP TABLE IF EXISTS ShowSeats;
DROP TABLE IF EXISTS Shows;
DROP TABLE IF EXISTS Seats;
DROP TABLE IF EXISTS Halls;
DROP TABLE IF EXISTS Movies;
DROP TABLE IF EXISTS Users;
GO

-- ============================================================================
-- Step 3: Create Tables
-- ============================================================================

-- 1. USERS (Admin, Staff, Customer)
CREATE TABLE Users (
    UserID        INT PRIMARY KEY IDENTITY(1,1),
    Username      VARCHAR(50)   NOT NULL UNIQUE,
    PasswordHash  VARCHAR(256)  NOT NULL,
    Role          VARCHAR(20)   NOT NULL CHECK (Role IN ('Admin', 'Customer')),
    FullName      NVARCHAR(100) NOT NULL,
    Phone         VARCHAR(20),
    Email         VARCHAR(100),
    CreatedDate   DATETIME      DEFAULT GETDATE()
);

-- 2. MOVIES
CREATE TABLE Movies (
    MovieID          INT PRIMARY KEY IDENTITY(1,1),
    Title            NVARCHAR(200) NOT NULL,
    Genre            VARCHAR(50),
    DurationMinutes  INT           NOT NULL,
    Language         VARCHAR(30),
    AgeRating        VARCHAR(10),
    ReleaseDate      DATE,
    Description      NVARCHAR(MAX)
);

-- 3. HALLS
CREATE TABLE Halls (
    HallID      INT PRIMARY KEY IDENTITY(1,1),
    HallName    NVARCHAR(50) NOT NULL,
    TotalSeats  INT          NOT NULL
);

-- 4. SEATS (physical seats in each hall)
CREATE TABLE Seats (
    SeatID    INT PRIMARY KEY IDENTITY(1,1),
    HallID    INT         NOT NULL,
    SeatRow   CHAR(1)     NOT NULL,
    SeatNo    INT         NOT NULL,
    SeatType  VARCHAR(20) DEFAULT 'Regular',
    CONSTRAINT FK_Seats_Halls FOREIGN KEY (HallID) REFERENCES Halls(HallID) ON DELETE CASCADE,
    CONSTRAINT UQ_Seat_In_Hall UNIQUE (HallID, SeatRow, SeatNo)
);

-- 5. SHOWS (movie scheduled in a hall at a specific date/time)
CREATE TABLE Shows (
    ShowID       INT PRIMARY KEY IDENTITY(1,1),
    MovieID      INT           NOT NULL,
    HallID       INT           NOT NULL,
    ShowDate     DATE          NOT NULL,
    ShowTime     TIME          NOT NULL,
    TicketPrice  DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_Shows_Movies FOREIGN KEY (MovieID) REFERENCES Movies(MovieID),
    CONSTRAINT FK_Shows_Halls  FOREIGN KEY (HallID)  REFERENCES Halls(HallID)
);

-- 6. SHOWSEATS (pre-generated seat availability per show)
CREATE TABLE ShowSeats (
    ShowSeatID  INT PRIMARY KEY IDENTITY(1,1),
    ShowID      INT         NOT NULL,
    SeatID      INT         NOT NULL,
    Status      VARCHAR(20) DEFAULT 'Available'
                CHECK (Status IN ('Available', 'Locked', 'Booked')),
    CONSTRAINT FK_ShowSeats_Shows FOREIGN KEY (ShowID) REFERENCES Shows(ShowID) ON DELETE CASCADE,
    CONSTRAINT FK_ShowSeats_Seats FOREIGN KEY (SeatID) REFERENCES Seats(SeatID),
    CONSTRAINT UQ_ShowSeat        UNIQUE (ShowID, SeatID)
);

-- 7. BOOKINGS
CREATE TABLE Bookings (
    BookingID    INT PRIMARY KEY IDENTITY(1,1),
    UserID       INT           NOT NULL,
    ShowID       INT           NOT NULL,
    BookingDate  DATETIME      DEFAULT GETDATE(),
    TotalAmount  DECIMAL(10,2) NOT NULL,
    Status       VARCHAR(20)   DEFAULT 'Confirmed'
                 CHECK (Status IN ('Confirmed', 'Cancelled')),
    CONSTRAINT FK_Bookings_Users FOREIGN KEY (UserID) REFERENCES Users(UserID),
    CONSTRAINT FK_Bookings_Shows FOREIGN KEY (ShowID) REFERENCES Shows(ShowID)
);

-- 8. BOOKINGSEATS (junction: seats associated with each booking)
CREATE TABLE BookingSeats (
    BookingSeatID  INT PRIMARY KEY IDENTITY(1,1),
    BookingID      INT NOT NULL,
    ShowSeatID     INT NOT NULL,
    CONSTRAINT FK_BookingSeats_Bookings  FOREIGN KEY (BookingID)  REFERENCES Bookings(BookingID) ON DELETE CASCADE,
    CONSTRAINT FK_BookingSeats_ShowSeats FOREIGN KEY (ShowSeatID) REFERENCES ShowSeats(ShowSeatID)
);

-- 9. PAYMENTS
CREATE TABLE Payments (
    PaymentID      INT PRIMARY KEY IDENTITY(1,1),
    BookingID      INT           NOT NULL,
    Amount         DECIMAL(10,2) NOT NULL,
    PaymentMethod  VARCHAR(30)   NOT NULL
                   CHECK (PaymentMethod IN ('Cash', 'Card', 'MobileBanking', 'Mobile Banking')),
    PaymentDate    DATETIME      DEFAULT GETDATE(),
    Status         VARCHAR(20)   DEFAULT 'Success'
                   CHECK (Status IN ('Success', 'Failed', 'Refunded')),
    CONSTRAINT FK_Payments_Bookings FOREIGN KEY (BookingID) REFERENCES Bookings(BookingID) ON DELETE CASCADE
);

-- 10. FOOD ITEMS
CREATE TABLE FoodItems (
    FoodItemID  INT PRIMARY KEY IDENTITY(1,1),
    Name        NVARCHAR(100)  NOT NULL,
    Price       DECIMAL(10,2)  NOT NULL,
    Category    VARCHAR(50)
);

-- 11. FOOD ORDERS
CREATE TABLE FoodOrders (
    FoodOrderID  INT PRIMARY KEY IDENTITY(1,1),
    BookingID    INT           NOT NULL,
    OrderDate    DATETIME      DEFAULT GETDATE(),
    TotalAmount  DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_FoodOrders_Bookings FOREIGN KEY (BookingID) REFERENCES Bookings(BookingID) ON DELETE CASCADE
);

-- 12. FOOD ORDER ITEMS
CREATE TABLE FoodOrderItems (
    FoodOrderItemID  INT PRIMARY KEY IDENTITY(1,1),
    FoodOrderID      INT           NOT NULL,
    FoodItemID       INT           NOT NULL,
    Quantity         INT           NOT NULL,
    Subtotal         DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_FoodOrderItems_FoodOrders FOREIGN KEY (FoodOrderID) REFERENCES FoodOrders(FoodOrderID) ON DELETE CASCADE,
    CONSTRAINT FK_FoodOrderItems_FoodItems  FOREIGN KEY (FoodItemID)  REFERENCES FoodItems(FoodItemID)
);
GO

-- ============================================================================
-- Step 4: Create Indexes for Performance
-- ============================================================================
CREATE INDEX IX_Seats_HallID             ON Seats(HallID);
CREATE INDEX IX_Shows_MovieID            ON Shows(MovieID);
CREATE INDEX IX_Shows_HallID             ON Shows(HallID);
CREATE INDEX IX_Shows_ShowDate           ON Shows(ShowDate);
CREATE INDEX IX_ShowSeats_ShowID         ON ShowSeats(ShowID);
CREATE INDEX IX_ShowSeats_SeatID         ON ShowSeats(SeatID);
CREATE INDEX IX_ShowSeats_Status         ON ShowSeats(Status);
CREATE INDEX IX_Bookings_UserID          ON Bookings(UserID);
CREATE INDEX IX_Bookings_ShowID          ON Bookings(ShowID);
CREATE INDEX IX_BookingSeats_BookingID   ON BookingSeats(BookingID);
CREATE INDEX IX_Payments_BookingID       ON Payments(BookingID);
CREATE INDEX IX_FoodOrders_BookingID     ON FoodOrders(BookingID);
CREATE INDEX IX_FoodOrderItems_OrderID   ON FoodOrderItems(FoodOrderID);
GO

PRINT '✓ Tables and Indexes created successfully.';
GO

-- ============================================================================
-- Step 5: Seed Data
-- ============================================================================

-- --- A. SEED USERS ---
-- Pre-hashed with SHA256 + salt format used by CinemaHallSystem.Utilities.PasswordHasher
INSERT INTO Users (Username, PasswordHash, Role, FullName, Phone, Email) VALUES
('admin',    '1iTLRqt2UtOfXfMHlWsIig==:hh/xGuX44/87sQp3vDKT066dIrtw4zTrrz9UUA2HgoM=', 'Admin',    'System Administrator', '01711000001', 'admin@cinema.com'),
('customer', '8Nbx+WvHoS+eMvWkoq/JTw==:tiz/pYYBdzmeW447stGPYUeem1qUQbvBQ5Y26W3sq0k=', 'Customer', 'John Doe',             '01711000003', 'customer@cinema.com');

-- --- B. SEED HALLS ---
INSERT INTO Halls (HallName, TotalSeats) VALUES
('Hall A - Gold',     80),
('Hall B - Silver',   48),
('Hall C - Platinum', 60),
('Hall D (Royal)',    30);

-- --- C. SEED SEATS ---
-- Hall 1: Rows A-H (8 rows) x 10 seats = 80 seats (Rows A & B Premium, C-H Regular)
INSERT INTO Seats (HallID, SeatRow, SeatNo, SeatType)
SELECT 1, R.Letter, N.Num,
       CASE WHEN R.Letter IN ('A','B') THEN 'Premium' ELSE 'Regular' END
FROM (VALUES ('A'),('B'),('C'),('D'),('E'),('F'),('G'),('H')) AS R(Letter)
CROSS JOIN (VALUES (1),(2),(3),(4),(5),(6),(7),(8),(9),(10)) AS N(Num);

-- Hall 2: Rows A-F (6 rows) x 8 seats = 48 seats (All Regular)
INSERT INTO Seats (HallID, SeatRow, SeatNo, SeatType)
SELECT 2, R.Letter, N.Num, 'Regular'
FROM (VALUES ('A'),('B'),('C'),('D'),('E'),('F')) AS R(Letter)
CROSS JOIN (VALUES (1),(2),(3),(4),(5),(6),(7),(8)) AS N(Num);

-- Hall 3: Rows A-F (6 rows) x 10 seats = 60 seats (Row A Premium, B-F Regular)
INSERT INTO Seats (HallID, SeatRow, SeatNo, SeatType)
SELECT 3, R.Letter, N.Num,
       CASE WHEN R.Letter = 'A' THEN 'Premium' ELSE 'Regular' END
FROM (VALUES ('A'),('B'),('C'),('D'),('E'),('F')) AS R(Letter)
CROSS JOIN (VALUES (1),(2),(3),(4),(5),(6),(7),(8),(9),(10)) AS N(Num);

-- Hall 4: Rows A-E (5 rows) x 6 seats = 30 seats (All Royal)
INSERT INTO Seats (HallID, SeatRow, SeatNo, SeatType)
SELECT 4, R.Letter, N.Num, 'Royal'
FROM (VALUES ('A'),('B'),('C'),('D'),('E')) AS R(Letter)
CROSS JOIN (VALUES (1),(2),(3),(4),(5),(6)) AS N(Num);

-- --- D. SEED MOVIES (2 movies per genre across 6 genres) ---
INSERT INTO Movies (Title, Genre, DurationMinutes, Language, AgeRating, ReleaseDate, Description) VALUES
-- Action
('The Dark Knight', 'Action', 152, 'English', 'PG-13', '2008-07-18',
 'When the menace known as the Joker wreaks havoc on Gotham, Batman must accept one of the greatest tests to fight injustice.'),
('John Wick: Chapter 4', 'Action', 169, 'English', 'R', '2023-03-24',
 'John Wick uncovers a path to defeating The High Table. But before he can earn his freedom, he must face a new enemy.'),

-- Sci-Fi
('Inception', 'Sci-Fi', 148, 'English', 'PG-13', '2010-07-16',
 'A thief who steals corporate secrets through dream-sharing technology is given the task of planting an idea into the mind of a CEO.'),
('Interstellar', 'Sci-Fi', 169, 'English', 'PG-13', '2014-11-07',
 'A team of explorers travel through a wormhole in space in an attempt to ensure humanity''s survival.'),

-- Thriller
('Parasite', 'Thriller', 132, 'Korean', 'R', '2019-05-30',
 'Greed and class discrimination threaten the newly formed symbiotic relationship between the wealthy Park family and the destitute Kim clan.'),
('Gone Girl', 'Thriller', 149, 'English', 'R', '2014-10-03',
 'With his wife''s disappearance having become the focus of an intense media circus, a man sees the spotlight turned on him.'),

-- Comedy
('The Grand Budapest Hotel', 'Comedy', 99, 'English', 'R', '2014-03-28',
 'A writer encounters the owner of an aging high-class hotel, who tells him of his early years serving as a lobby boy.'),
('Superbad', 'Comedy', 113, 'English', 'R', '2007-08-17',
 'Two co-dependent high school seniors are forced to deal with separation anxiety after their plan to stage a booze-fueled party goes awry.'),

-- Drama
('The Shawshank Redemption', 'Drama', 142, 'English', 'R', '1994-09-23',
 'Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.'),
('Forrest Gump', 'Drama', 142, 'English', 'PG-13', '1994-07-06',
 'The story of a man with a low IQ who accomplished great things in his life, and was present during several historic events.'),

-- Horror
('Get Out', 'Horror', 104, 'English', 'R', '2017-02-24',
 'A young African-American visits his white girlfriend''s parents for the weekend, where his simmering uneasiness about their reception reaches a boiling point.'),
('A Quiet Place', 'Horror', 90, 'English', 'PG-13', '2018-04-06',
 'In a post-apocalyptic world, a family is forced to live in silence while hiding from monsters with ultra-sensitive hearing.');

-- --- E. SEED SHOWS (Dynamically scheduled over upcoming days) ---
DECLARE @mDarkKnight INT = (SELECT TOP 1 MovieID FROM Movies WHERE Title = 'The Dark Knight');
DECLARE @mInception  INT = (SELECT TOP 1 MovieID FROM Movies WHERE Title = 'Inception');
DECLARE @mParasite   INT = (SELECT TOP 1 MovieID FROM Movies WHERE Title = 'Parasite');
DECLARE @mBudapest   INT = (SELECT TOP 1 MovieID FROM Movies WHERE Title = 'The Grand Budapest Hotel');
DECLARE @mShawshank  INT = (SELECT TOP 1 MovieID FROM Movies WHERE Title = 'The Shawshank Redemption');
DECLARE @mGetOut     INT = (SELECT TOP 1 MovieID FROM Movies WHERE Title = 'Get Out');
DECLARE @mJohnWick   INT = (SELECT TOP 1 MovieID FROM Movies WHERE Title = 'John Wick: Chapter 4');

INSERT INTO Shows (MovieID, HallID, ShowDate, ShowTime, TicketPrice) VALUES
(@mDarkKnight, 1, CAST(GETDATE() AS DATE),                     '18:00', 400.00),
(@mInception,  2, CAST(GETDATE() AS DATE),                     '19:30', 350.00),
(@mJohnWick,   1, CAST(DATEADD(DAY, 1, GETDATE()) AS DATE),    '14:00', 350.00),
(@mParasite,   3, CAST(DATEADD(DAY, 1, GETDATE()) AS DATE),    '18:00', 300.00),
(@mBudapest,   2, CAST(DATEADD(DAY, 2, GETDATE()) AS DATE),    '16:00', 300.00),
(@mShawshank,  1, CAST(DATEADD(DAY, 2, GETDATE()) AS DATE),    '20:00', 450.00),
(@mGetOut,     3, CAST(DATEADD(DAY, 3, GETDATE()) AS DATE),    '21:00', 350.00),
-- Hall D (Royal) Shows at ৳1500
(@mDarkKnight, 4, CAST(GETDATE() AS DATE),                     '21:00', 1500.00),
(@mJohnWick,   4, CAST(DATEADD(DAY, 1, GETDATE()) AS DATE),    '18:30', 1500.00),
(@mInception,  4, CAST(DATEADD(DAY, 2, GETDATE()) AS DATE),    '20:00', 1500.00);

-- --- F. PRE-GENERATE SHOWSEATS ---
-- Pre-generate one ShowSeat row for each Seat in each Show's assigned Hall
INSERT INTO ShowSeats (ShowID, SeatID, Status)
SELECT s.ShowID, st.SeatID, 'Available'
FROM Shows s
INNER JOIN Seats st ON st.HallID = s.HallID;

-- --- G. SEED FOOD ITEMS ---
INSERT INTO FoodItems (Name, Price, Category) VALUES
('Large Popcorn',      250.00, 'Snacks'),
('Small Popcorn',      150.00, 'Snacks'),
('Nachos with Cheese', 200.00, 'Snacks'),
('Hot Dog',            180.00, 'Meals'),
('Coca Cola',          100.00, 'Beverages'),
('Mineral Water',       50.00, 'Beverages');
GO

-- ============================================================================
-- Step 6: Confirmation Summary
-- ============================================================================
PRINT '====================================================================';
PRINT '🎉 DATABASE SETUP COMPLETE!';
PRINT '====================================================================';
SELECT 'Users' AS [Table], COUNT(*) AS [RecordCount] FROM Users
UNION ALL
SELECT 'Halls', COUNT(*) FROM Halls
UNION ALL
SELECT 'Seats', COUNT(*) FROM Seats
UNION ALL
SELECT 'Movies', COUNT(*) FROM Movies
UNION ALL
SELECT 'Shows', COUNT(*) FROM Shows
UNION ALL
SELECT 'ShowSeats (Available for Booking)', COUNT(*) FROM ShowSeats
UNION ALL
SELECT 'FoodItems', COUNT(*) FROM FoodItems;

PRINT '';
PRINT 'DEFAULT LOGIN CREDENTIALS:';
PRINT '  - Admin:    Username: admin    | Password: admin123';
PRINT '  - Customer: Username: customer | Password: customer123';
PRINT '====================================================================';
GO
