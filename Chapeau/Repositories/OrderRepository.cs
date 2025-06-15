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
            string query = "INSERT INTO [Order] (employeeID, tableID, orderTime, isPaid) " +
                           "VALUES (@employeeID, @tableID, @orderTime, 0); " +
                           "SELECT SCOPE_IDENTITY();";
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@employeeID", order.Employee.employeeID);
                    command.Parameters.AddWithValue("@tableID", order.Table.TableId);
                    command.Parameters.AddWithValue("@orderTime", order.OrderTime);

                    connection.Open();
                    object result = command.ExecuteScalar();
                    order.OrderId = Convert.ToInt32(result);
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error while inserting order.", ex);
            }

        }

        //get active order by table to double check that if there is any existent order, avoiding to creating everytime a new
        public Order? GetActiveOrderByTableId(int tableId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = @"
                               SELECT * FROM [Order]
                               WHERE TableId = @tableId AND IsPaid = 0";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@tableId", tableId);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        return MapToOrder(reader);
                    }
                    return null;
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error while retrieving active order.", ex);
            }
        }
        private Order MapToOrder(SqlDataReader reader)
        {
            int tableId = Convert.ToInt32(reader["tableID"]);
            Table matchedTable = _tableRepository.GetTableById(tableId);

            return new Order
            {
                OrderId = Convert.ToInt32(reader["orderID"]),
                OrderTime = Convert.ToDateTime(reader["orderTime"]),
                IsPaid = Convert.ToBoolean(reader["isPaid"]),
                Table = matchedTable,
                Employee = _employeeRepository.GetEmployeeByID(Convert.ToInt32(reader["employeeID"])),
            };
        }
    }
}

