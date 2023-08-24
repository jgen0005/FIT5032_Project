-- Users Table
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(255) NOT NULL,
    LastName NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) UNIQUE NOT NULL,
    Password NVARCHAR(255) NOT NULL, -- This will store the hashed password
    Role NVARCHAR(50) CHECK (Role IN ('Patient', 'MedicalStaff')),
    DateRegistered DATETIME DEFAULT GETDATE(),
    ProfilePicture VARBINARY(MAX), -- Optional for storing image data
    ContactNumber NVARCHAR(50)
);

-- Appointments Table
CREATE TABLE Appointments (
    AppointmentID INT PRIMARY KEY IDENTITY(1,1),
    PatientID INT FOREIGN KEY REFERENCES Users(UserID),
    StaffID INT FOREIGN KEY REFERENCES Users(UserID),
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

-- Notifications Table
CREATE TABLE Notifications (
    NotificationID INT PRIMARY KEY IDENTITY(1,1),
    RecipientUserID INT FOREIGN KEY REFERENCES Users(UserID),
    MessageContent NVARCHAR(MAX) NOT NULL,
    DateSent DATETIME DEFAULT GETDATE(),
    Status NVARCHAR(50) CHECK (Status IN ('Unread', 'Read')),
    Attachment VARBINARY(MAX) -- Optional for storing attachments
);

-- BulkEmails Table
CREATE TABLE BulkEmails (
    EmailID INT PRIMARY KEY IDENTITY(1,1),
    Subject NVARCHAR(255) NOT NULL,
    Body NVARCHAR(MAX) NOT NULL,
    DateSent DATETIME DEFAULT GETDATE()
);

-- EmailRecipients Junction Table
CREATE TABLE EmailRecipients (
    EmailID INT FOREIGN KEY REFERENCES BulkEmails(EmailID),
    UserID INT FOREIGN KEY REFERENCES Users(UserID),
    PRIMARY KEY (EmailID, UserID)
);
