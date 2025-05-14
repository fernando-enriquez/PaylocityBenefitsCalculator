using Api.Models;

namespace Api.Data
{
    public class DbSeeder
    {
        public static void Seed(EmployeePaycheckDbContext context)
        {
            if (!context.Employees.Any())
            {
                context.Employees.AddRange(new[]
                {
                    new Employee
                    {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Salary = 73500
                    },
                    new Employee
                    {
                    Id = 2,
                    FirstName = "Annie",
                    LastName = "Gray",
                    Salary = 87000
                    },
                    new Employee
                    {
                    Id = 3,
                    FirstName = "Taylor",
                    LastName = "Mckenzie",
                    Salary = 90000
                    },
                    new Employee
                    {
                    Id = 4,
                    FirstName = "Jorge",
                    LastName = "Perales",
                    Salary = 82500
                    },
                    new Employee
                    {
                        Id = 5,
                        FirstName = "Laura",
                        LastName = "Sanchez",
                        Salary = 85000,
                        Dependents = new List<Dependent>
                        {
                            new Dependent
                            {
                                FirstName = "peter",
                                LastName = "Stevenson",
                                DateOfBirth = new DateTime(1973, 01, 05),
                                Relationship = Relationship.Spouse
                            },
                            new Dependent
                            {
                                FirstName = "Lucía",
                                LastName = "Stevenson",
                                DateOfBirth = new DateTime(2012, 10, 01),
                                Relationship = Relationship.Child
                            }
                        }
                    }
                });

                context.SaveChanges();
            }

            if (!context.Rules.Any())
            {
                context.Rules.AddRange(new[]
                {
                    new Rule { Id = 1, Concept = "domesticPartnerQuantityLimit", Value = 1m, Type = "Fixed", Description = "Max number of dosmestic partner or spouse allowed per employee" },
                    new Rule { Id = 2, Concept = "childrenQuantityLimit", Unlimited = true, Type = "Fixed" },
                    new Rule { Id = 3, Concept = "employeeBaseCost", Value = 1000m, Type = "Fixed" },
                    new Rule { Id = 4, Concept = "dependentBaseCost", Value = 600m, Type = "Fixed" },
                    new Rule { Id = 5, Concept = "paychecksPerYear", Value = 26m, Type = "Fixed" },
                    new Rule { Id = 6, Concept = "amountForAdditionalSalaryFee", Value = 80000m, Type = "Fixed" },
                    new Rule { Id = 7, Concept = "additionalSalaryFee", Value = 2m, Type = "Percent" },
                    new Rule { Id = 8, Concept = "YearsForAdditionalAgeFee", Value = 50m, Type = "Fixed" },
                    new Rule { Id = 9, Concept = "extraAgeFee", Value = 200m, Type = "Fixed" }
                });
            }

            context.SaveChanges();
        }
    }
}
