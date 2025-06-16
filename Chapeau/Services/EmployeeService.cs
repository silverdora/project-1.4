using Chapeau.Models;
using Chapeau.Repository.Interface;
using Chapeau.Service.Interface;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;

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
            string hashed = HashPassword(plainPassword);

            return _employeeRepository.GetByLoginCredentials(userName, hashed);
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashBytes);
            }
        }


    }
}

