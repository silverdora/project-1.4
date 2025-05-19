using Chapeau.Models;
using System.Collections.Generic;

namespace Chapeau.Repository.Interface
{
    public interface IEmployeeRepository
    {
        List<Employee> GetAllEmployee();
        Employee? GetByLoginCredentials(string userName, string _ignoredPassword);
        Employee? GetEmployeeByID(int employeeId);
    }
}