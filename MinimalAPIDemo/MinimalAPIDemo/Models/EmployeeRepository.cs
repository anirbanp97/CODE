namespace MinimalAPIDemo.Models
{
    //public class EmployeeRepository : IEmployeeRepository
    //{
    //    // Internal list simulating persistent storage
    //    private readonly List<Employee> _employeeList;
    //    public EmployeeRepository()
    //    {
    //        // Initialize with sample employees to provide initial data
    //        _employeeList = new List<Employee>
    //        {
    //            new Employee { Id = 1, Name = "John Doe", Position = "Software Engineer", Salary = 60000 },
    //            new Employee { Id = 2, Name = "Jane Smith", Position = "Project Manager", Salary = 80000 }
    //        };
    //    }
    //    // Returns the entire list of employees
    //    public List<Employee> GetAllEmployees()
    //    {
    //        return _employeeList;
    //    }
    //    // Finds an employee by their unique ID or returns null if not found
    //    public Employee? GetEmployeeById(int id)
    //    {
    //        return _employeeList.FirstOrDefault(e => e.Id == id);
    //    }
    //    // Adds a new employee, automatically assigning a unique ID
    //    public Employee AddEmployee(Employee newEmployee)
    //    {
    //        // Assign ID as max existing ID + 1, or 1 if list is empty
    //        newEmployee.Id = _employeeList.Count > 0 ? _employeeList.Max(emp => emp.Id) + 1 : 1;
    //        _employeeList.Add(newEmployee);
    //        return newEmployee;
    //    }
    //    // Updates an existing employee; returns updated employee or null if not found
    //    public Employee? UpdateEmployee(int id, Employee updatedEmployee)
    //    {
    //        var employee = _employeeList.FirstOrDefault(emp => emp.Id == id);
    //        if (employee == null)
    //            return null;
    //        // Update fields individually to avoid replacing the whole object
    //        employee.Name = updatedEmployee.Name;
    //        employee.Position = updatedEmployee.Position;
    //        employee.Salary = updatedEmployee.Salary;
    //        return employee;
    //    }
    //    // Deletes employee by ID; returns true if deletion succeeded, else false
    //    public bool DeleteEmployee(int id)
    //    {
    //        var employee = _employeeList.FirstOrDefault(emp => emp.Id == id);
    //        if (employee == null)
    //            return false;
    //        _employeeList.Remove(employee);
    //        return true;
    //    }
    //}
    private readonly List<Employee> _employeeList = new List<Employee>
        {
            new Employee { Id = 1, Name = "John Doe", Position = "Software Engineer", Salary = 60000 },
            new Employee { Id = 2, Name = "Jane Smith", Position = "Project Manager", Salary = 80000 }
        };
    public async Task<List<Employee>> GetAllEmployeesAsync()
    {
        //Simulate database call
        await Task.Delay(TimeSpan.FromSeconds(1));
        return _employeeList;
    }
    public async Task<Employee?> GetEmployeeByIdAsync(int id)
    {
        //Simulate database call
        await Task.Delay(TimeSpan.FromSeconds(1));
        var employee = _employeeList.FirstOrDefault(e => e.Id == id);
        return employee;
    }
    public async Task<Employee> AddEmployeeAsync(Employee newEmployee)
    {
        //Simulate database call
        await Task.Delay(TimeSpan.FromSeconds(1));
        newEmployee.Id = _employeeList.Count > 0 ? _employeeList.Max(e => e.Id) + 1 : 1;
        _employeeList.Add(newEmployee);
        return newEmployee;
    }
    public async Task<Employee?> UpdateEmployeeAsync(int id, Employee updatedEmployee)
    {
        //Simulate database call
        await Task.Delay(TimeSpan.FromSeconds(1));
        var employee = _employeeList.FirstOrDefault(e => e.Id == id);
        if (employee == null)
            return null;
        employee.Name = updatedEmployee.Name;
        employee.Position = updatedEmployee.Position;
        employee.Salary = updatedEmployee.Salary;
        return employee;
    }
    public async Task<bool> DeleteEmployeeAsync(int id)
    {
        //Simulate database call
        await Task.Delay(TimeSpan.FromSeconds(1));
        var employee = _employeeList.FirstOrDefault(e => e.Id == id);
        if (employee == null)
            return false;
        _employeeList.Remove(employee);
        return true;
    }
}
}
