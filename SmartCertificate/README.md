Smart Certificate Verification System

This is a simple console-based demo project that demonstrates:
- OOP concepts: classes, inheritance (User, Student, Admin, Employer)
- Method overloading for certificate verification
- Exception handling
- Arrays, searching, sorting, LINQ queries
- File handling with StreamReader/StreamWriter and FileStream
- SQL Server integration with basic CRUD operations

Requirements
- Visual Studio 2025/2026
- .NET 10 SDK
- SQL Server / LocalDB (default uses (localdb)\\MSSQLLocalDB)

How to run
1. Open the solution `SmartCertificate.sln` in Visual Studio.
2. (Optional) Edit `appsettings.txt` in the project root to set a different connection string.
3. Build and run the project. The console menu will demonstrate CRUD and verification features.

Authentication demo
- Use menu option `User Menu -> Register` to register a user. Provide role as `Student`, `Admin`, or `Employer`.
- Use `User Menu -> Login` to login and `User Menu -> Logout` to logout. The `AuthService` stores the current user in memory.

Project structure
- `Models/` (User, Student, Admin, Employer, Certificate, Course, Transcript)
- `Services/` (AuthService, StudentService, CertificateService, TranscriptService)
- `Data/` (DatabaseManager / Database)
- `Utils/` (FileHelper, FileConfig, FileManager)
- `Program.cs` - entry point and console UI

Notes
- SQL script available in `Scripts/CreateDatabase.sql` if you prefer to create the DB manually.
- Sample data available in `SampleData/certificates.csv`.
