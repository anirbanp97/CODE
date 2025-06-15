
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
            // -------------------- Define Minimal API Endpoints with try-catch --------------------
            // GET /employees - Fetch all employees
            app.MapGet("/employees", (IEmployeeRepository repo, ILogger<Program> logger, HttpContext httpContext) =>
            {
                try
                {
                    logger.LogInformation("Fetching all employees");
                    // Example: Simulate error if query parameter "causeError" is true
                    var query = httpContext.Request.Query;
                    if (query.ContainsKey("causeError") &&
                    bool.TryParse(query["causeError"], out bool causeError) && causeError)
                    {
                        // Simulate a null reference exception
                        throw new NullReferenceException("Simulated null reference exception for testing.");
                    }
                    var employees = repo.GetAllEmployees();
                    return Results.Ok(employees);  // Return HTTP 200 with employee list
                }
                catch (Exception ex)
                {
                    // Log exception details as Error level
                    logger.LogError(ex, "Error occurred while fetching all employees");
                    // Return a consistent HTTP 500 problem JSON response
                    return Results.Problem(
                    detail: "An error occurred while processing your request.",
                    statusCode: 500,
                    instance: httpContext.Request.Path,
                    title: "Internal Server Error");
                }
            });
            // GET /employees/{id} - Fetch employee by ID
            app.MapGet("/employees/{id}", (int id, IEmployeeRepository repo, ILogger<Program> logger) =>
            {
                try
                {
                    logger.LogInformation($"Fetching employee with ID: {id}");
                    var employee = repo.GetEmployeeById(id);
                    if (employee == null)
                    {
                        // Log warning if employee not found
                        logger.LogWarning($"Employee with ID {id} not found");
                        // Return HTTP 404 with message
                        return Results.NotFound(new { Message = $"Employee with ID {id} not found." });
                    }
                    return Results.Ok(employee);  // Return HTTP 200 with employee details
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Error occurred while fetching employee with ID {id}");
                    return Results.Problem(
                    detail: "An error occurred while processing your request.",
                    statusCode: 500,
                    title: "Internal Server Error");
                }
            });
            // POST /employees - Create a new employee
            app.MapPost("/employees", (Employee newEmployee, IEmployeeRepository repo, ILogger<Program> logger) =>
            {
                try
                {
                    // Validate incoming employee data
                    if (!ValidationHelper.TryValidate(newEmployee, out var errors))
                    {
                        logger.LogWarning($"Validation failed for new employee: {string.Join(", ", errors.Select(e => e.ErrorMessage))}");
                        // Return HTTP 400 Bad Request with validation errors
                        return Results.BadRequest(new
                        {
                            Message = "Validation Failed",
                            Errors = errors.Select(e => e.ErrorMessage)
                        });
                    }
                    // Add the new employee record
                    var createdEmployee = repo.AddEmployee(newEmployee);
                    logger.LogInformation($"Employee created with ID {createdEmployee.Id}");
                    // Return HTTP 201 Created with location header
                    return Results.Created($"/employees/{createdEmployee.Id}", createdEmployee);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred while creating a new employee");
                    return Results.Problem(
                    detail: "An error occurred while processing your request.",
                    statusCode: 500,
                    title: "Internal Server Error");
                }
            });
            // PUT /employees/{id} - Update an existing employee
            app.MapPut("/employees/{id}", (int id, Employee updatedEmployee, IEmployeeRepository repo, ILogger<Program> logger) =>
            {
                try
                {
                    // Validate updated data
                    if (!ValidationHelper.TryValidate(updatedEmployee, out var errors))
                    {
                        logger.LogWarning($"Validation failed while updating employee {id}: {string.Join(", ", errors.Select(e => e.ErrorMessage))}");
                        return Results.BadRequest(new
                        {
                            Message = "Validation Failed",
                            Errors = errors.Select(e => e.ErrorMessage)
                        });
                    }
                    // Update employee if exists
                    var employee = repo.UpdateEmployee(id, updatedEmployee);
                    if (employee == null)
                    {
                        logger.LogWarning($"Attempted to update non-existent employee with ID {id}");
                        return Results.NotFound(new { Message = $"Employee with ID {id} not found." });
                    }
                    logger.LogInformation($"Employee with ID {id} updated");
                    return Results.Ok(employee);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Error occurred while updating employee with ID {id}");
                    return Results.Problem(
                    detail: "An error occurred while processing your request.",
                    statusCode: 500,
                    title: "Internal Server Error");
                }
            });
            // DELETE /employees/{id} - Delete an employee by ID
            app.MapDelete("/employees/{id}", (int id, IEmployeeRepository repo, ILogger<Program> logger) =>
            {
                try
                {
                    var deleted = repo.DeleteEmployee(id);
                    if (!deleted)
                    {
                        logger.LogWarning($"Attempted to delete non-existent employee with ID {id}");
                        return Results.NotFound(new { Message = $"Employee with ID {id} not found." });
                    }
                    logger.LogInformation("Employee with ID {EmployeeId} deleted", id);
                    return Results.NoContent();  // HTTP 204 No Content on successful deletion
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Error occurred while deleting employee with ID {id}");
                    return Results.Problem(
                    detail: "An error occurred while processing your request.",
                    statusCode: 500,
                    title: "Internal Server Error");
                }
            });
            // Start the web server and listen for incoming HTTP requests
            app.Run();
        }
    }
}