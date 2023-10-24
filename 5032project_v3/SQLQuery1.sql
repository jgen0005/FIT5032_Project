-- Appointments Table
CREATE TABLE Appointments (
    AppointmentID INT PRIMARY KEY IDENTITY(1,1),
    PatientID NVARCHAR(128) FOREIGN KEY REFERENCES AspNetUsers(Id),
    StaffID NVARCHAR(128) FOREIGN KEY REFERENCES AspNetUsers(Id),
    DateOfAppointment DATE NOT NULL,
    TimeSlot NVARCHAR(50) NOT NULL,
    Status NVARCHAR(50) CHECK (Status IN ('Scheduled', 'Completed', 'Cancelled')),
    FeedbackRating INT CHECK (FeedbackRating BETWEEN 1 AND 5),
    FeedbackComment NVARCHAR(MAX)
);

-- XRayRecords Table
CREATE TABLE XRayRecords (
    RecordID INT PRIMARY KEY IDENTITY(1,1),
    AppointmentID INT FOREIGN KEY REFERENCES Appointments(AppointmentID),
    XRayImage VARBINARY(MAX) NOT NULL,
    DateTaken DATE NOT NULL,
    Description NVARCHAR(MAX),
    Diagnosis NVARCHAR(MAX)
);


