using Microsoft.Data.SqlClient;
using Chapeau.Models;
using Chapeau.Repository.Interface;
using System.Data;

namespace Chapeau.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly string _connectionString;

        public EmployeeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<Employee> GetAllEmployee()
        {
            List<Employee> employees = new List<Employee>();
            string query = "SELECT employeeID, employee_name, employee_role FROM dbo.Employee";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        employees.Add(new Employee
                        {
                            employeeID = Convert.ToInt32(reader["employeeID"]),
                            employeeName = reader["employee_name"].ToString(),
                            Role = (Role)Enum.Parse(typeof(Role), reader["employee_role"].ToString())
                        });
                    }
                }
            }

            return employees;
        }

        public Employee? GetByLoginCredentials(string userName, string password)
        {
            string query = "SELECT employeeID, employee_name, employee_role FROM dbo.Employee WHERE username = @username AND password = @password";

            SqlParameter[] sqlParameters = new SqlParameter[]
            {
        new SqlParameter("@username", SqlDbType.NVarChar) { Value = userName },
        new SqlParameter("@password", SqlDbType.NVarChar) { Value = password }
            };

            return ExecuteQueryMapEmployee(query, sqlParameters);
        }


        public Employee? GetEmployeeByID(int employeeId)
        {
            string query = "SELECT employeeID, employee_name, employee_role FROM dbo.Employee WHERE employeeID = @employeeID";

            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@employeeID", SqlDbType.Int) { Value = employeeId }
            };

            return ExecuteQueryMapEmployee(query, sqlParameters);
        }

        private Employee? ExecuteQueryMapEmployee(string query, SqlParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddRange(parameters);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Employee
                        {
                            employeeID = Convert.ToInt32(reader["employeeID"]),
                            employeeName = reader["employee_name"].ToString(),
                            Role = (Role)Enum.Parse(typeof(Role), reader["employee_role"].ToString()),
                            //Password = reader["password"] != DBNull.Value ? reader["password"].ToString() : null
                        };
                    }
                }
            }

            return null;
        }
    }
}