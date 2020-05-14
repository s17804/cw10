ALTER TABLE Student
    ADD Password VARCHAR(255),
        Refresh_Token VARCHAR(255),
        Salt VARBINARY(32);

