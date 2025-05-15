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
                    Salary = 73500,
                    DateOfBirth = new DateTime(1985, 4, 20)
                },
                new Employee
                {
                    Id = 2,
                    FirstName = "Annie",
                    LastName = "Gray",
                    Salary = 87000,
                    DateOfBirth = new DateTime(1990, 9, 15),
                    Dependents = new List<Dependent>
                    {
                        new Dependent
                        {
                            FirstName = "Ben",
                            LastName = "Gray",
                            DateOfBirth = new DateTime(1970, 2, 10), // > 50
                            Relationship = Relationship.DomesticPartner
                        }
                    }
                },
                new Employee
                {
                    Id = 3,
                    FirstName = "Taylor",
                    LastName = "Mckenzie",
                    Salary = 90000,
                    DateOfBirth = new DateTime(1978, 11, 8),
                    Dependents = new List<Dependent>
                    {
                        new Dependent
                        {
                            FirstName = "Emma",
                            LastName = "Mckenzie",
                            DateOfBirth = new DateTime(2015, 7, 25),
                            Relationship = Relationship.Child
                        },
                        new Dependent
                        {
                            FirstName = "Logan",
                            LastName = "Mckenzie",
                            DateOfBirth = new DateTime(2012, 4, 30),
                            Relationship = Relationship.Child
                        }
                    }
                },
                new Employee
                {
                    Id = 4,
                    FirstName = "Jorge",
                    LastName = "Perales",
                    Salary = 82500,
                    DateOfBirth = new DateTime(1988, 6, 10)
                },
                new Employee
                {
                    Id = 5,
                    FirstName = "Laura",
                    LastName = "Sanchez",
                    Salary = 85000,
                    DateOfBirth = new DateTime(1986, 3, 5),
                    Dependents = new List<Dependent>
                    {
                        new Dependent
                        {
                            FirstName = "Peter",
                            LastName = "Stevenson",
                            DateOfBirth = new DateTime(1973, 1, 5), // > 50
                            Relationship = Relationship.Spouse
                        },
                        new Dependent
                        {
                            FirstName = "Lucía",
                            LastName = "Stevenson",
                            DateOfBirth = new DateTime(2012, 10, 1),
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
                    new Rule { Id = 1, Concept = "domesticPartnerQuantityLimit", Value = 1m, Description = "Max number of dosmestic partner or spouse allowed per employee" },
                    new Rule { Id = 2, Concept = "childrenQuantityLimit", Unlimited = true },
                    new Rule { Id = 3, Concept = "employeeBaseCost", Value = 1000m  },
                    new Rule { Id = 4, Concept = "dependentBaseCost", Value = 600m },
                    new Rule { Id = 5, Concept = "paychecksPerYear", Value = 26m },
                    new Rule { Id = 6, Concept = "thresholdAnnualSalary", Value = 80000m },
                    new Rule { Id = 7, Concept = "thresholdAnnualSalaryFee", Value = 2m },
                    new Rule { Id = 8, Concept = "thresholdAge", Value = 50m },
                    new Rule { Id = 9, Concept = "thresholdAgeFee", Value = 200m }
                });
            }

            context.SaveChanges();
        }
    }
}
