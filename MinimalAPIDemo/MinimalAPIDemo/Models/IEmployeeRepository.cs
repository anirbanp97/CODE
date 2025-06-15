namespace MinimalAPIDemo.Models
{
    public interface IEmployeeRepository
    {
        //// Retrieve all employees
        //List<Employee> GetAllEmployees();
        //// Retrieve a single employee by their ID, or null if not found
        //Employee? GetEmployeeById(int id);
        //// Add a new employee and return the created employee with assigned ID
        //Employee AddEmployee(Employee employee);
        //// Update an existing employee by ID, return updated employee or null if not found
        //Employee? UpdateEmployee(int id, Employee updatedEmployee);
        //// Delete an employee by ID, returns true if deleted successfully, false otherwise
        //bool DeleteEmployee(int id);

        Task<List<Employee>> GetAllEmployeesAsync();
        Task<Employee?> GetEmployeeByIdAsync(int id);
        Task<Employee> AddEmployeeAsync(Employee employee);
        Task<Employee?> UpdateEmployeeAsync(int id, Employee updatedEmployee);
        Task<bool> DeleteEmployeeAsync(int id);
    }
}
