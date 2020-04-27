﻿INSERT INTO Studies (IdStudy, Name)
VALUES (1, 'Informatyka dzienne'),
       (2, 'Informatyka zaoczne'),
       (3, 'Informatyka internetowe'),
       (4, 'Sztuka nowych mediów dzienne'),
       (5, 'Sztukaz nowych mediów zaoczne');

INSERT INTO Enrollment (IdEnrollment, Semester, IdStudy, StartDate)
VALUES (1, 1, 1, '2020-03-29'),
       (2, 1, 2, '2020-03-29'),
       (3, 1, 3, '2020-03-29'),
       (4, 1, 4, '2020-03-29'),
       (5, 1, 5, '2020-03-29'),
       (6, 1, 2, '2019-03-29');

INSERT INTO Student (IndexNumber, FirstName, LastName, BirthDate, IdEnrollment)
VALUES ('s11111', 'Adam', 'Arbuz', '2000-05-19', 1),
       ('s11112', 'Beata', 'Brokuł', '2001-01-26', 2),
       ('s11113', 'Cecylia', 'Cebula', '1999-08-12', 3),
       ('s11114', 'Dominik', 'Dynia', '2000-12-29', 4),
       ('s11115', 'Edward', 'Eukaliptus', '2002-03-15', 5);