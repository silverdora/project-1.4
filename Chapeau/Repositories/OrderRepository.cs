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

        //get active order by table to double check that if there is any existent order, avoiding to creating everytime a new
        public Order? GetActiveOrderByTableId(int tableId)
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
                    // Map to Order object
                    return MapToOrder(reader);
                }

                return null;
            }
        }
        private Order MapToOrder(SqlDataReader reader)
        {
            int tableId = Convert.ToInt32(reader["tableID"]);
            Table matchedTable = null;
            List<Table> allTables = _tableRepository.GetAllTables();

            foreach (Table table in allTables)
            {
                if (table.TableId == tableId)
                {
                    matchedTable = table;
                    break;
                }
            }

            return new Order
            {
                OrderID = Convert.ToInt32(reader["orderID"]),
                OrderTime = Convert.ToDateTime(reader["orderTime"]),
                IsPaid = Convert.ToBoolean(reader["isPaid"]),
                Table = matchedTable,
                Employee = _employeeRepository.GetEmployeeByID(Convert.ToInt32(reader["employeeID"])),
                //Notes = reader["notes"]?.ToString()
            };
        }
    }
}

