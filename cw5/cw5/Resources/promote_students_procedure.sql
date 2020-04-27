CREATE PROCEDURE PromoteStudents(@Semester INT, @Studies VARCHAR(255))
AS
BEGIN
    BEGIN
        TRANSACTION tran_1;
    DECLARE @IdStudies INT = (SELECT S.IdStudy FROM Studies S WHERE S.Name = @Studies);
    IF @IdStudies IS NULL
        BEGIN
            ROLLBACK;
            RAISERROR ('404 Not Found', 1, 1);
            RETURN;
        END

    DECLARE @IdEnrollment INT = (SELECT E.IdEnrollment
                                 FROM Enrollment E
                                 WHERE E.Semester = @Semester
                                   AND E.IdStudy = @IdStudies);
    IF @IdEnrollment IS NULL
        BEGIN
            ROLLBACK;
            RAISERROR ('404 Not Found', 1, 1);
            RETURN;
        END

    DECLARE @IdEnrollmentPromoted INT = (SELECT E.IdEnrollment FROM Enrollment E WHERE E.Semester = (@Semester + 1));
    IF @IdEnrollmentPromoted IS NULL
        BEGIN
            DECLARE @TEMP_TABLE TABLE
                          (
                              TEMP_ID INT
                          )
            INSERT INTO Enrollment(IdEnrollment, Semester, IdStudy, StartDate)
            OUTPUT INSERTED.IdEnrollment INTO @TEMP_TABLE
            VALUES ((SELECT MAX(E.IdEnrollment) FROM Enrollment E) + 1, @Semester + 1, @IdStudies, GETDATE());
            SELECT @IdEnrollmentPromoted = TEMP_ID from @TEMP_TABLE;
        END
        
    UPDATE Student SET IdEnrollment = @IdEnrollmentPromoted WHERE IdEnrollment = @IdEnrollment
    COMMIT;
    SELECT * FROM Enrollment E WHERE E.IdEnrollment = @IdEnrollmentPromoted;
END
    