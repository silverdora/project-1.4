using Chapeau.Models;
using Chapeau.Repository.Interface;
using Chapeau.Service.Interface;
using Microsoft.AspNetCore.Identity;

namespace Chapeau.Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public List<Employee> GetAllEmployee()
        {
            return _employeeRepository.GetAllEmployee();
        }

        public Employee? GetEmployeeByID(int employeeId)
        {
            return _employeeRepository.GetEmployeeByID(employeeId);
        }

        public Employee? GetByLoginCredentials(string userName, string plainPassword)
        {
            return _employeeRepository.GetByLoginCredentials(userName, plainPassword);
        }

    }
}

