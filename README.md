This project uses SQLite as its database engine for simplicity and portability.

You don't need to manually create or configure the database. Upon running the project for the first time, the database will be automatically:

Created (if it doesn't exist)

Migrated using EF Core migrations

Seeded with sample data (employees, dependents, rules)

This is handled in Program.cs:

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EmployeePaycheckDbContext>();
    db.Database.Migrate(); // Apply EF Core migrations
    DbSeeder.Seed(db);     // Seed initial data
}
The database file is created as EmployeePaycheck.db and stored in the project root
