using System.Collections.Generic;
using System.Reflection.Emit;

namespace MinimalAPIDemo.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Employee> Employees { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Seed initial Employee data
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = 1,
                    Name = "John Doe",
                    Position = "Software Engineer",
                    Salary = 60000m
                },
                new Employee
                {
                    Id = 2,
                    Name = "Jane Smith",
                    Position = "Project Manager",
                    Salary = 80000m
                }
            );
        }

        internal async Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
