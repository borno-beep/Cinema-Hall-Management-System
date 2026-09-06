-- ============================================================
-- Cinema Hall Management System — Database Schema
-- Target: SQL Server Express (.\SQLEXPRESS)
-- Run this script FIRST, then run SeedData.sql
-- ============================================================

-- Create the database if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'CinemaHallDB')
    CREATE DATABASE CinemaHallDB;
GO

USE CinemaHallDB;
GO

-- Drop tables in reverse dependency order (safe re-run)
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

-- ============================================================
-- USERS — stores all app users (Admin, Staff, Customer)
-- ============================================================
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

-- ============================================================
-- MOVIES
-- ============================================================
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

-- ============================================================
-- HALLS
-- ============================================================
CREATE TABLE Halls (
    HallID      INT PRIMARY KEY IDENTITY(1,1),
    HallName    NVARCHAR(50) NOT NULL,
    TotalSeats  INT          NOT NULL
);

-- ============================================================
-- SEATS — physical seats in a hall (row + number)
-- ============================================================
CREATE TABLE Seats (
    SeatID    INT PRIMARY KEY IDENTITY(1,1),
    HallID    INT       NOT NULL,
    SeatRow   CHAR(1)   NOT NULL,
    SeatNo    INT        NOT NULL,
    SeatType  VARCHAR(20) DEFAULT 'Regular',
    CONSTRAINT FK_Seats_Halls FOREIGN KEY (HallID) REFERENCES Halls(HallID) ON DELETE CASCADE,
    CONSTRAINT UQ_Seat_In_Hall UNIQUE (HallID, SeatRow, SeatNo)
);

-- ============================================================
-- SHOWS — a movie scheduled in a hall at a date/time
-- ============================================================
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

-- ============================================================
-- SHOWSEATS — one row per seat per show; Status tracks availability
-- This is the KEY design decision: pre-generating seats per show
-- allows O(1) status lookup and supports "Locked" state during checkout.
-- ============================================================
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

-- ============================================================
-- BOOKINGS
-- ============================================================
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

-- ============================================================
-- BOOKINGSEATS — junction: which ShowSeats belong to which Booking
-- ============================================================
CREATE TABLE BookingSeats (
    BookingSeatID  INT PRIMARY KEY IDENTITY(1,1),
    BookingID      INT NOT NULL,
    ShowSeatID     INT NOT NULL,
    CONSTRAINT FK_BookingSeats_Bookings  FOREIGN KEY (BookingID)  REFERENCES Bookings(BookingID) ON DELETE CASCADE,
    CONSTRAINT FK_BookingSeats_ShowSeats FOREIGN KEY (ShowSeatID) REFERENCES ShowSeats(ShowSeatID)
);

-- ============================================================
-- PAYMENTS
-- ============================================================
CREATE TABLE Payments (
    PaymentID      INT PRIMARY KEY IDENTITY(1,1),
    BookingID      INT           NOT NULL,
    Amount         DECIMAL(10,2) NOT NULL,
    PaymentMethod  VARCHAR(30)   NOT NULL
                   CHECK (PaymentMethod IN ('Cash', 'Card', 'MobileBanking')),
    PaymentDate    DATETIME      DEFAULT GETDATE(),
    Status         VARCHAR(20)   DEFAULT 'Success'
                   CHECK (Status IN ('Success', 'Failed', 'Refunded')),
    CONSTRAINT FK_Payments_Bookings FOREIGN KEY (BookingID) REFERENCES Bookings(BookingID) ON DELETE CASCADE
);

-- ============================================================
-- FOOD ITEMS — menu catalog
-- ============================================================
CREATE TABLE FoodItems (
    FoodItemID  INT PRIMARY KEY IDENTITY(1,1),
    Name        NVARCHAR(100)  NOT NULL,
    Price       DECIMAL(10,2)  NOT NULL,
    Category    VARCHAR(50)
);

-- ============================================================
-- FOOD ORDERS — one order per booking
-- ============================================================
CREATE TABLE FoodOrders (
    FoodOrderID  INT PRIMARY KEY IDENTITY(1,1),
    BookingID    INT           NOT NULL,
    OrderDate    DATETIME      DEFAULT GETDATE(),
    TotalAmount  DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_FoodOrders_Bookings FOREIGN KEY (BookingID) REFERENCES Bookings(BookingID) ON DELETE CASCADE
);

-- ============================================================
-- FOOD ORDER ITEMS — junction: which FoodItems in which FoodOrder
-- ============================================================
CREATE TABLE FoodOrderItems (
    FoodOrderItemID  INT PRIMARY KEY IDENTITY(1,1),
    FoodOrderID      INT           NOT NULL,
    FoodItemID       INT           NOT NULL,
    Quantity         INT           NOT NULL,
    Subtotal         DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_FoodOrderItems_FoodOrders FOREIGN KEY (FoodOrderID) REFERENCES FoodOrders(FoodOrderID) ON DELETE CASCADE,
    CONSTRAINT FK_FoodOrderItems_FoodItems  FOREIGN KEY (FoodItemID)  REFERENCES FoodItems(FoodItemID)
);

-- ============================================================
-- INDEXES for query performance
-- ============================================================
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

PRINT '✓ Schema created successfully.';
GO
