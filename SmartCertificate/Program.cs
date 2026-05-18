using System;
using System.Linq;
using SmartCertificate.Models;
using SmartCertificate.Services;
using SmartCertificate.Repositories;
using SmartCertificate.Data;
using SmartCertificate.Utils;

namespace SmartCertificate
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Smart Certificate Verification System - Console Demo");

            // Configure connection string - update with your SQL Server details or use LocalDB
            var connectionString = FileConfig.LoadConnectionString();
            var db = new Database(connectionString);
            var repo = new CertificateRepository(db);
            var service = new CertificateService(repo);

            // User management
            var userRepo = new Repositories.UserRepository(db);
            var auth = new Services.AuthService(userRepo);

            try
            {
                userRepo.EnsureTable();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Warning: Could not ensure users table. " + ex.Message);
            }

            // Create table if not exists (script provided in Scripts/CreateDatabase.sql)
            try
            {
                repo.EnsureTable();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Warning: Could not ensure database table. " + ex.Message);
            }

            // Load sample data from CSV into in-memory list
            var samples = FileHelper.ReadCertificatesFromCsv("SampleData/certificates.csv");
            foreach (var c in samples)
            {
                try { repo.Add(c); } catch { /* ignore duplicate inserts for demo */ }
            }

            // Main interactive loop: certificate and user management
            while (true)
            {
                try
                {
                    Console.WriteLine();
                    Console.WriteLine("Main Menu: 1=Certificates 2=Users 9=Exit");
                    Console.Write("Choose: ");
                    var choice = Console.ReadLine()?.Trim();
                    switch (choice)
                    {
                        case "1":
                            CertificateMenu(repo, service);
                            break;
                        case "2":
                            UserMenu(userRepo, auth, repo, service);
                            break;
                        case "9":
                            return;
                        default:
                            Console.WriteLine("Invalid choice");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }

        static void CertificateMenu(CertificateRepository repo, CertificateService service)
        {
            Console.WriteLine("Certificate Menu: 1=List 2=Add 3=Update 4=Delete 5=VerifyById 6=VerifyBySerial 7=Search/Sort 8=ExportLogs 9=Back");
            Console.Write("Choose: ");
            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    var all = repo.GetAll();
                    foreach (var cert in all)
                        Console.WriteLine(cert);
                    break;
                case "2":
                    var newCert = new Certificate { SerialNumber = Guid.NewGuid().ToString().Substring(0,8), HolderName = "New Student", IssueDate = DateTime.UtcNow, ExpiryDate = DateTime.UtcNow.AddYears(1) };
                    repo.Add(newCert);
                    Console.WriteLine("Added: " + newCert);
                    break;
                case "3":
                    Console.Write("Enter Id to update: ");
                    if (int.TryParse(Console.ReadLine(), out int uid))
                    {
                        var cert = repo.GetById(uid);
                        if (cert != null)
                        {
                            cert.HolderName += " (Updated)";
                            repo.Update(cert);
                            Console.WriteLine("Updated.");
                        }
                        else Console.WriteLine("Not found.");
                    }
                    break;
                case "4":
                    Console.Write("Enter Id to delete: ");
                    if (int.TryParse(Console.ReadLine(), out int did)) { repo.Delete(did); Console.WriteLine("Deleted (if existed)."); }
                    break;
                case "5":
                    Console.Write("Enter Id to verify: ");
                    if (int.TryParse(Console.ReadLine(), out int vid))
                    {
                        var res = service.VerifyCertificate(vid);
                        Console.WriteLine("Verification: " + res);
                    }
                    break;
                case "6":
                    Console.Write("Enter Serial to verify: ");
                    var s = Console.ReadLine();
                    var res2 = service.VerifyCertificate(s);
                    Console.WriteLine("Verification: " + res2);
                    break;
                case "7":
                    var list = repo.GetAll();
                    Console.WriteLine("Total: " + list.Count);
                    var arr = list.ToArray();
                    Array.Sort(arr, (a,b) => DateTime.Compare(a.IssueDate, b.IssueDate));
                    Console.WriteLine("Sorted by IssueDate:");
                    foreach (var c in arr) Console.WriteLine(c);
                    var recent = list.Where(x => x.ExpiryDate > DateTime.UtcNow).OrderBy(x => x.HolderName).Take(5);
                    Console.WriteLine("Recent (LINQ):");
                    foreach (var c in recent) Console.WriteLine(c);
                    break;
                case "8":
                    FileHelper.WriteLog("logs.txt", "Exported certificates at " + DateTime.UtcNow);
                    Console.WriteLine("Exported logs to logs.txt");
                    break;
                case "9":
                    return;
                default:
                    Console.WriteLine("Invalid choice");
                    break;
            }
        }

        static void UserMenu(Repositories.UserRepository userRepo, Services.AuthService auth, CertificateRepository certRepo, CertificateService certService)
        {
            Console.WriteLine("User Menu: 1=Register 2=Login 3=Logout 4=RoleAssign 5=UsersList 6=DeleteUser 7=Dashboard 9=Back");
            Console.Write("Choose: ");
            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    try
                    {
                        Console.Write("Username: "); var u = Console.ReadLine();
                        Console.Write("Email: "); var e = Console.ReadLine();
                        Console.Write("Password: "); var p = Console.ReadLine();
                        Console.Write("Confirm Password: "); var cp = Console.ReadLine();
                        if (p != cp) { Console.WriteLine("Passwords do not match"); break; }
                        if (string.IsNullOrWhiteSpace(u) || string.IsNullOrWhiteSpace(p)) { Console.WriteLine("Invalid input"); break; }
                        var role = "Student";
                        var res = auth.Register(u.Trim(), e?.Trim(), p, role);
                        Console.WriteLine(res);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                    break;
                case "2":
                    Console.Write("Username: "); var lu = Console.ReadLine();
                    Console.Write("Password: "); var lp = Console.ReadLine();
                    var lres = auth.Login(lu?.Trim(), lp);
                    Console.WriteLine(lres == "Success" ? $"Logged in as {auth.CurrentUser.Username} ({auth.CurrentUser.Role})" : lres);
                    break;
                case "3":
                    auth.Logout(); Console.WriteLine("Logged out");
                    break;
                case "4":
                    // Role assignment - only Admin can assign
                    if (auth.CurrentUser == null || auth.CurrentUser.Role != "Admin") { Console.WriteLine("Only Admin can assign roles"); break; }
                    Console.Write("Target username: "); var tu = Console.ReadLine();
                    Console.Write("New role (Student/Admin/Employer): "); var nr = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(tu) || string.IsNullOrWhiteSpace(nr)) { Console.WriteLine("Invalid input"); break; }
                    userRepo.UpdateRole(tu.Trim(), nr.Trim());
                    Console.WriteLine("Role updated");
                    break;
                case "5":
                    // List users (Admin only)
                    if (auth.CurrentUser == null || auth.CurrentUser.Role != "Admin") { Console.WriteLine("Admin only"); break; }
                    var users = userRepo.GetAll();
                    foreach (var usr in users) Console.WriteLine($"[{usr.Id}] {usr.Username} - {usr.Email} - {usr.Role}");
                    break;
                case "6":
                    if (auth.CurrentUser == null || auth.CurrentUser.Role != "Admin") { Console.WriteLine("Admin only"); break; }
                    Console.Write("Username to delete: "); var du = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(du)) { userRepo.DeleteByUsername(du.Trim()); Console.WriteLine("Deleted (if existed)"); }
                    break;
                case "7":
                    if (auth.CurrentUser == null) { Console.WriteLine("Login required"); break; }
                    ShowDashboard(auth.CurrentUser, certRepo, certService);
                    break;
                case "9":
                    return;
                default:
                    Console.WriteLine("Invalid choice");
                    break;
            }
        }

        static void ShowDashboard(Models.User user, CertificateRepository certRepo, CertificateService certService)
        {
            Console.WriteLine($"Dashboard for {user.Username} ({user.Role})");
            switch (user.Role)
            {
                case "Admin":
                    Console.WriteLine("Admin can manage users and certificates.");
                    break;
                case "Student":
                    Console.WriteLine("Student can view and verify their certificates.");
                    break;
                case "Employer":
                    Console.WriteLine("Employer can verify certificates for hiring.");
                    break;
                default:
                    Console.WriteLine("General user.");
                    break;
            }
        }
    }
}
