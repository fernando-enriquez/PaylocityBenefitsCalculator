using Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Api.Data
{
    public class EmployeePaycheckDbContext : DbContext
    {
        public EmployeePaycheckDbContext(DbContextOptions<EmployeePaycheckDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Dependent> Dependents { get; set; }
        public DbSet<Models.Rule> Rules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relaciones adicionales si quieres
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Dependents)
                .WithOne(d => d.Employee)
                .HasForeignKey(d => d.EmployeeId);
        }
    }
}
