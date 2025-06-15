using Microsoft.EntityFrameworkCore;
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
            builder.Services.AddMemoryCache();   // Add memory cache service

            // Configure JSON serialization options for HTTP responses
            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                // Disable camelCase conversion; keep property names as declared (PascalCase)
                options.SerializerOptions.PropertyNamingPolicy = null;
            });

            // Add Swagger/OpenAPI support services for API documentation and UI generation
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register your repositories and filters
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<CachingEndPointFilter>();
            builder.Services.AddScoped<ValidationEndPointFilter<Employee>>();
            builder.Services.AddScoped<LoggingEndPointFilter>();

            //Register ApplicationDbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Register the custom global error handling middleware in the pipeline
            // It will catch exceptions from downstream middleware/endpoints
            app.UseMiddleware<ErrorHandlerMiddleware>();

            // -------------------- Define Minimal API Endpoints with try-catch --------------------

            // GET /employees - Fetch all employees
            app.MapGet("/employees", async (IEmployeeRepository repo, ILogger<Program> logger) =>
            {
                logger.LogInformation("Fetching all employees asynchronously");
                var employees = await repo.GetAllEmployeesAsync();
                return Results.Ok(employees);
            })
            .AddEndpointFilter<LoggingEndPointFilter>() //Log the Request Timing
            .AddEndpointFilter<CachingEndPointFilter>(); // Cache entire employee list for 30 seconds;

            // GET /employees/{id} - Fetch employee by ID
            app.MapGet("/employees/{id}", async (int id, IEmployeeRepository repo, ILogger<Program> logger) =>
            {
                logger.LogInformation($"Fetching employee with ID: {id} asynchronously");
                var employee = await repo.GetEmployeeByIdAsync(id);
                return employee is not null ? Results.Ok(employee) : Results.NotFound();
            })
            .AddEndpointFilter<LoggingEndPointFilter>()  //Log the Request Timing
            .AddEndpointFilter<CachingEndPointFilter>(); // Cache individual employee responses;

            // POST /employees - Create a new employee
            app.MapPost("/employees", async (Employee newEmployee, IEmployeeRepository repo, ILogger<Program> logger) =>
            {
                //No need to write the Custom Validation logic here
                var createdEmployee = await repo.AddEmployeeAsync(newEmployee);
                logger.LogInformation($"Employee created with ID {createdEmployee.Id} asynchronously");
                return Results.Created($"/employees/{createdEmployee.Id}", createdEmployee);
            })
            .AddEndpointFilter<LoggingEndPointFilter>() //Log the Request Timing
            .AddEndpointFilter<ValidationEndPointFilter<Employee>>(); // Validate input

            // PUT /employees/{id} - Update an existing employee
            app.MapPut("/employees/{id}", async (int id, Employee updatedEmployee, IEmployeeRepository repo, ILogger<Program> logger) =>
            {
                var employee = await repo.UpdateEmployeeAsync(id, updatedEmployee);
                return employee is not null ? Results.Ok(employee) : Results.NotFound();
            })
            .AddEndpointFilter<LoggingEndPointFilter>() //Log the Request Timing
            .AddEndpointFilter<ValidationEndPointFilter<Employee>>(); // Validate input

            // DELETE /employees/{id} - Delete an employee by ID
            app.MapDelete("/employees/{id}", async (int id, IEmployeeRepository repo, ILogger<Program> logger) =>
            {
                var deleted = await repo.DeleteEmployeeAsync(id);
                return deleted ? Results.NoContent() : Results.NotFound();
            })
            .AddEndpointFilter<LoggingEndPointFilter>(); //Log the Request Timing

            app.Run();
        }
    }
}