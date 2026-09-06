-- ============================================================
-- Add 2 movies per genre
-- Run this in SSMS against CinemaHallDB
-- ============================================================

USE CinemaHallDB;
GO

-- Clear existing movies (optional — remove these 2 lines if you want to KEEP existing movies)
DELETE FROM ShowSeats WHERE ShowID IN (SELECT ShowID FROM Shows);
DELETE FROM Shows;
DELETE FROM Movies;

-- ============================================================
-- ACTION (2 movies)
-- ============================================================
INSERT INTO Movies (Title, Genre, DurationMinutes, Language, AgeRating, ReleaseDate, Description) VALUES
('The Dark Knight', 'Action', 152, 'English', 'PG-13', '2008-07-18',
 'When the menace known as the Joker wreaks havoc on Gotham, Batman must accept one of the greatest tests to fight injustice.'),
('John Wick: Chapter 4', 'Action', 169, 'English', 'R', '2023-03-24',
 'John Wick uncovers a path to defeating The High Table. But before he can earn his freedom, he must face a new enemy.');

-- ============================================================
-- SCI-FI (2 movies)
-- ============================================================
INSERT INTO Movies (Title, Genre, DurationMinutes, Language, AgeRating, ReleaseDate, Description) VALUES
('Inception', 'Sci-Fi', 148, 'English', 'PG-13', '2010-07-16',
 'A thief who steals corporate secrets through dream-sharing technology is given the task of planting an idea into the mind of a CEO.'),
('Interstellar', 'Sci-Fi', 169, 'English', 'PG-13', '2014-11-07',
 'A team of explorers travel through a wormhole in space in an attempt to ensure humanity''s survival.');

-- ============================================================
-- THRILLER (2 movies)
-- ============================================================
INSERT INTO Movies (Title, Genre, DurationMinutes, Language, AgeRating, ReleaseDate, Description) VALUES
('Parasite', 'Thriller', 132, 'Korean', 'R', '2019-05-30',
 'Greed and class discrimination threaten the newly formed symbiotic relationship between the wealthy Park family and the destitute Kim clan.'),
('Gone Girl', 'Thriller', 149, 'English', 'R', '2014-10-03',
 'With his wife''s disappearance having become the focus of an intense media circus, a man sees the spotlight turned on him.');

-- ============================================================
-- COMEDY (2 movies)
-- ============================================================
INSERT INTO Movies (Title, Genre, DurationMinutes, Language, AgeRating, ReleaseDate, Description) VALUES
('The Grand Budapest Hotel', 'Comedy', 99, 'English', 'R', '2014-03-28',
 'A writer encounters the owner of an aging high-class hotel, who tells him of his early years serving as a lobby boy.'),
('Superbad', 'Comedy', 113, 'English', 'R', '2007-08-17',
 'Two co-dependent high school seniors are forced to deal with separation anxiety after their plan to stage a booze-fueled party goes awry.');

-- ============================================================
-- DRAMA (2 movies)
-- ============================================================
INSERT INTO Movies (Title, Genre, DurationMinutes, Language, AgeRating, ReleaseDate, Description) VALUES
('The Shawshank Redemption', 'Drama', 142, 'English', 'R', '1994-09-23',
 'Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.'),
('Forrest Gump', 'Drama', 142, 'English', 'PG-13', '1994-07-06',
 'The story of a man with a low IQ who accomplished great things in his life, and was present during several historic events.');

-- ============================================================
-- HORROR (2 movies)
-- ============================================================
INSERT INTO Movies (Title, Genre, DurationMinutes, Language, AgeRating, ReleaseDate, Description) VALUES
('Get Out', 'Horror', 104, 'English', 'R', '2017-02-24',
 'A young African-American visits his white girlfriend''s parents for the weekend, where his simmering uneasiness about their reception of him eventually reaches a boiling point.'),
('A Quiet Place', 'Horror', 90, 'English', 'PG-13', '2018-04-06',
 'In a post-apocalyptic world, a family is forced to live in silence while hiding from monsters with ultra-sensitive hearing.');

-- ============================================================
-- Now create shows for some of these movies
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

-- Auto-generate ShowSeats for all new shows
INSERT INTO ShowSeats (ShowID, SeatID, Status)
SELECT s.ShowID, st.SeatID, 'Available'
FROM Shows s
INNER JOIN Seats st ON st.HallID = s.HallID
WHERE NOT EXISTS (SELECT 1 FROM ShowSeats ss WHERE ss.ShowID = s.ShowID AND ss.SeatID = st.SeatID);

PRINT '✓ 12 movies (2 per genre) + 7 shows + ShowSeats inserted!';
GO
