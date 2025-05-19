using Chapeau.Models;

namespace Chapeau.Service.Interface
{
    public interface IEmployeeService
    {
        List<Employee> GetAllEmployee();
        Employee? GetEmployeeByID(int employeeId);
        Employee? GetByLoginCredentials(string userName, string plainPassword);
    }
}