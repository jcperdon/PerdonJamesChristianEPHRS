using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EPHRS
{
    class Program
    {
        static List<Employee> employees = new List<Employee>();   // starts empty
        static int nextId = 1001;

        // Step 4 (merge conflict): edit THIS line differently on main and your feature branch
        const string AppTitle = "EMPLOYEE PAYROLL & HR MANAGEMENT SYSTEM";

        static void Main()
        {
            bool running = true;
            while (running)
            {
                Console.WriteLine($"\n===== {AppTitle} =====");
                Console.WriteLine("1. Create Employee");
                Console.WriteLine("2. Read (View All)");
                Console.WriteLine("3. Search / Filter");
                Console.WriteLine("4. Update Employee");
                Console.WriteLine("5. Delete Employee");
                Console.WriteLine("0. Exit");
                Console.Write("Choose: ");

                switch (Console.ReadLine()?.Trim())
                {
                    case "1": CreateEmployee(); break;
                    case "2": ReadAll(); break;
                    case "3": SearchMenu(); break;
                    case "4": UpdateEmployee(); break;
                    case "5": DeleteEmployee(); break;
                    case "0": running = false; break;
                    default: Console.WriteLine("Invalid choice."); break;
                }
            }
        }

        // ---------- CREATE ----------
        static void CreateEmployee()
        {
            var emp = new Employee
            {
                Id = nextId++,
                FirstName = ReadString("First name: "),
                LastName = ReadString("Last name: "),
                Department = ReadString("Department (IT, HR, Sales, Finance): "),
                JobTitle = ReadString("Job title: "),
                MonthlySalary = ReadDecimal("Monthly salary: "),
                TaxRate = ReadTaxRate("Tax rate (e.g., 0.12 for 12%): "),
                HireDate = ReadDate("Hire date (yyyy-MM-dd): "),
                IsActive = ReadBool("Active? (y/n): "),
                Email = ReadEmail("Email: ")
            };
            employees.Add(emp);
            Console.WriteLine($"Employee created with Id {emp.Id}.");
        }

        // ---------- READ ----------
        static void ReadAll()
        {
            if (!employees.Any()) { Console.WriteLine("No records."); return; }
            PrintList(employees.OrderBy(e => e.Id));
        }

        // ---------- SEARCH / FILTER ----------
        static void SearchMenu()
        {
            Console.WriteLine("\n1. By name  2. By department  3. By status  4. By salary range");
            Console.Write("Choose: ");
            IEnumerable<Employee> result;

            switch (Console.ReadLine()?.Trim())
            {
                case "1":
                    string name = ReadString("Name keyword: ");
                    result = employees.Where(e =>
                        e.FirstName.Contains(name, StringComparison.OrdinalIgnoreCase) ||
                        e.LastName.Contains(name, StringComparison.OrdinalIgnoreCase));
                    break;
                case "2":
                    string dept = ReadString("Department: ");
                    result = employees.Where(e =>
                        e.Department.Equals(dept, StringComparison.OrdinalIgnoreCase));
                    break;
                case "3":
                    bool active = ReadBool("Active employees? (y = active, n = inactive): ");
                    result = employees.Where(e => e.IsActive == active);
                    break;
                case "4":
                    decimal min = ReadDecimal("Min salary: ");
                    decimal max = ReadDecimal("Max salary: ");
                    result = employees.Where(e => e.MonthlySalary >= min && e.MonthlySalary <= max);
                    break;
                default:
                    Console.WriteLine("Invalid choice."); return;
            }

            var list = result.OrderBy(e => e.LastName).ToList();
            if (!list.Any()) Console.WriteLine("No matches found.");
            else PrintList(list);
        }

        // ---------- UPDATE ----------
        static void UpdateEmployee()
        {
            int id = ReadInt("Enter Id to update: ");
            var emp = employees.FirstOrDefault(e => e.Id == id);
            if (emp == null) { Console.WriteLine("Employee not found."); return; }

            Console.WriteLine("Press Enter to keep the current value.");
            string s;

            Console.Write($"First name [{emp.FirstName}]: ");
            if (!string.IsNullOrWhiteSpace(s = Console.ReadLine())) emp.FirstName = s.Trim();

            Console.Write($"Last name [{emp.LastName}]: ");
            if (!string.IsNullOrWhiteSpace(s = Console.ReadLine())) emp.LastName = s.Trim();

            Console.Write($"Department [{emp.Department}]: ");
            if (!string.IsNullOrWhiteSpace(s = Console.ReadLine())) emp.Department = s.Trim();

            Console.Write($"Job title [{emp.JobTitle}]: ");
            if (!string.IsNullOrWhiteSpace(s = Console.ReadLine())) emp.JobTitle = s.Trim();

            Console.Write($"Monthly salary [{emp.MonthlySalary}]: ");
            if (decimal.TryParse(Console.ReadLine(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal sal) && sal >= 0)
                emp.MonthlySalary = sal;

            Console.Write($"Tax rate [{emp.TaxRate}]: ");
            if (double.TryParse(Console.ReadLine(), NumberStyles.Float, CultureInfo.InvariantCulture, out double tax) && tax >= 0 && tax <= 1)
                emp.TaxRate = tax;

            Console.Write($"Active? y/n [{(emp.IsActive ? "y" : "n")}]: ");
            s = Console.ReadLine()?.Trim().ToLower();
            if (s == "y") emp.IsActive = true; else if (s == "n") emp.IsActive = false;

            Console.Write($"Email [{emp.Email}]: ");
            s = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(s) && s.Contains("@")) emp.Email = s.Trim();

            Console.WriteLine("Employee updated.");
        }

        // ---------- DELETE ----------
        static void DeleteEmployee()
        {
            int id = ReadInt("Enter Id to delete: ");
            var emp = employees.FirstOrDefault(e => e.Id == id);
            if (emp == null) { Console.WriteLine("Employee not found."); return; }

            if (ReadBool($"Delete {emp.FirstName} {emp.LastName}? (y/n): "))
            {
                employees.RemoveAll(e => e.Id == id);
                Console.WriteLine("Employee deleted.");
            }
            else Console.WriteLine("Delete cancelled.");
        }

        // ---------- DISPLAY ----------
        static void PrintList(IEnumerable<Employee> list)
        {
            Console.WriteLine($"\n{"Id",-6}{"First",-10}{"Last",-10}{"Dept",-9}{"Title",-18}{"Salary",12}{"Tax",6}{"Hired",-12}{"Active",-8}{"Net Pay",12}  Email");
            Console.WriteLine(new string('-', 130));
            foreach (var e in list)
            {
                Console.WriteLine($"{e.Id,-6}{e.FirstName,-10}{e.LastName,-10}{e.Department,-9}{e.JobTitle,-18}{e.MonthlySalary,12:N2}{e.TaxRate,6:P0}{e.HireDate:yyyy-MM-dd,-12}{(e.IsActive ? "Yes" : "No"),-8}{e.NetPay,12:N2}  {e.Email}");
            }
            Console.WriteLine($"Total records: {list.Count()}");
        }

        // ---------- INPUT HELPERS ----------
        static string ReadString(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string s = Console.ReadLine()?.Trim();
                if (!string.IsNullOrEmpty(s)) return s;
                Console.WriteLine("Value cannot be empty.");
            }
        }

        static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int n)) return n;
                Console.WriteLine("Enter a valid whole number.");
            }
        }

        static decimal ReadDecimal(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (decimal.TryParse(Console.ReadLine(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal d) && d >= 0)
                    return d;
                Console.WriteLine("Enter a valid non-negative amount.");
            }
        }

        static double ReadTaxRate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (double.TryParse(Console.ReadLine(), NumberStyles.Float, CultureInfo.InvariantCulture, out double d) && d >= 0 && d <= 1)
                    return d;
                Console.WriteLine("Enter a value between 0 and 1.");
            }
        }

        static DateTime ReadDate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime d))
                    return d;
                Console.WriteLine("Use the format yyyy-MM-dd.");
            }
        }

        static bool ReadBool(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string s = Console.ReadLine()?.Trim().ToLower();
                if (s == "y") return true;
                if (s == "n") return false;
                Console.WriteLine("Enter y or n.");
            }
        }

        static string ReadEmail(string prompt)
        {
            while (true)
            {
                string s = ReadString(prompt);
                if (s.Contains("@") && s.Contains(".")) return s;
                Console.WriteLine("Enter a valid email address.");
            }
        }
    }
}
