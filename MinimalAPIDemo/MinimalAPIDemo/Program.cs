
using MinimalAPIDemo.Models;
namespace MinimalAPIDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Create the WebApplication builder which prepares the app with default configs
            var builder = WebApplication.CreateBuilder(args);
            // Configure logging providers
            builder.Logging.ClearProviders();    // Remove any default logging providers
            builder.Logging.AddConsole();        // Add console logger (shows logs in terminal)
            builder.Logging.AddDebug();          // Add debug logger (for IDE/debugging tools)
            // Configure JSON serialization options for HTTP responses
            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                // Disable camelCase conversion; keep property names as declared (PascalCase)
                options.SerializerOptions.PropertyNamingPolicy = null;
            });
            // Add Swagger/OpenAPI support services for API documentation and UI generation
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            // Register the EmployeeRepository as a singleton service in DI container
            // This means one instance will be shared across the app lifetime
            builder.Services.AddSingleton<IEmployeeRepository, EmployeeRepository>();
            // Build the app (finalize service registration and middleware pipeline)
            var app = builder.Build();
            // Enable Swagger UI only in Development environment to test API endpoints interactively
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();      // Enable Swagger middleware to generate swagger.json
                app.UseSwaggerUI();    // Enable Swagger UI middleware to visualize API docs
            }
            // Register the custom global error handling middleware in the pipeline
            // It will catch exceptions from downstream middleware/endpoints
            app.UseMiddleware<ErrorHandlerMiddleware>();
            // -------------------- Define Minimal API Endpoints --------------------
            // GET /employees - Fetch all employees
            app.MapGet("/employees", async (IEmployeeRepository repo, ILogger<Program> logger) =>
            {
                logger.LogInformation("Fetching all employees asynchronously");
                var employees = await repo.GetAllEmployeesAsync();
                return Results.Ok(employees);
            });
            // GET /employees/{id} - Fetch employee by ID
            app.MapGet("/employees/{id}", async (int id, IEmployeeRepository repo, ILogger<Program> logger) =>
            {
                logger.LogInformation($"Fetching employee with ID: {id} asynchronously");
                var employee = await repo.GetEmployeeByIdAsync(id);
                return employee is not null ? Results.Ok(employee) : Results.NotFound();
            });
            // POST /employees - Create a new employee
            app.MapPost("/employees", async (Employee newEmployee, IEmployeeRepository repo, ILogger<Program> logger) =>
            {
                if (!ValidationHelper.TryValidate(newEmployee, out var errors))
                {
                    return Results.BadRequest(new
                    {
                        Message = "Validation Failed",
                        Errors = errors.Select(e => e.ErrorMessage)
                    });
                }
                var createdEmployee = await repo.AddEmployeeAsync(newEmployee);
                logger.LogInformation($"Employee created with ID {createdEmployee.Id} asynchronously");
                return Results.Created($"/employees/{createdEmployee.Id}", createdEmployee);
            });
            // PUT /employees/{id} - Update an existing employee
            app.MapPut("/employees/{id}", async (int id, Employee updatedEmployee, IEmployeeRepository repo, ILogger<Program> logger) =>
            {
                if (!ValidationHelper.TryValidate(updatedEmployee, out var errors))
                {
                    return Results.BadRequest(new
                    {
                        Message = "Validation Failed",
                        Errors = errors.Select(e => e.ErrorMessage)
                    });
                }
                var employee = await repo.UpdateEmployeeAsync(id, updatedEmployee);
                return employee is not null ? Results.Ok(employee) : Results.NotFound();
            });
            // DELETE /employees/{id} - Delete an employee by ID
            app.MapDelete("/employees/{id}", async (int id, IEmployeeRepository repo, ILogger<Program> logger) =>
            {
                var deleted = await repo.DeleteEmployeeAsync(id);
                return deleted ? Results.NoContent() : Results.NotFound();
            });
            // Start the web server and listen for incoming HTTP requests
            app.Run();
        }
    }
}