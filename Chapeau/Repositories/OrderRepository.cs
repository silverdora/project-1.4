using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using System.Data;
using Microsoft.Data.SqlClient;
using Chapeau.Repository.Interface;

namespace Chapeau.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;
        private readonly ITableRepository _tableRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public OrderRepository(IConfiguration configuration, ITableRepository tableRepository, IEmployeeRepository employeeRepository)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _tableRepository = tableRepository;
            _employeeRepository = employeeRepository;
        }

        public void InsertOrder(Order order)
        {
            string query = "INSERT INTO [Order] (employeeID, tableID, orderTime) " +
                           "VALUES (@employeeID, @tableID, @orderTime); " +
                           "SELECT SCOPE_IDENTITY();";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@employeeID", order.Employee.employeeID);
                command.Parameters.AddWithValue("@tableID", order.Table.TableId);
                command.Parameters.AddWithValue("@orderTime", order.OrderTime);

                connection.Open();
                object result = command.ExecuteScalar();
                order.OrderID = Convert.ToInt32(result);
            }
        }

       
    }
}

