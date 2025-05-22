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
         // ✅ Public method: insert + return full Order
        public Order TakeNewOrder(int tableId, Employee employee)
        {
            int newOrderID = InsertNewOrder(tableId, employee);
            return GetOrderByID(newOrderID);
        }

        // ✅ Public method to get Order by ID
        public Order GetOrderByID(int orderID)
        {
            string query = "SELECT * FROM [Order] WHERE orderID = @orderID";
            SqlParameter[] parameters = {
                new SqlParameter("@orderID", orderID)
            };

            return MapOrdersFromQuery(query, parameters).FirstOrDefault();
        }

        // ✅ Private helper to insert only
        private int InsertNewOrder(int tableId, Employee employee)
        {
            int newOrderID;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string insertQuery = @"
                    INSERT INTO [Order] (employeeID, tableID, order_time, isServed)
                    VALUES (@employeeID, @tableID, @orderTime, @isServed)";

                using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@employeeID", employee.employeeID);
                    insertCommand.Parameters.AddWithValue("@tableID", tableId);
                    insertCommand.Parameters.AddWithValue("@orderTime", DateTime.Now);
                    insertCommand.Parameters.AddWithValue("@isServed", false);
                    insertCommand.ExecuteNonQuery();
                }

                string selectIdQuery = "SELECT CAST(SCOPE_IDENTITY() AS int)";
                using (SqlCommand selectCommand = new SqlCommand(selectIdQuery, connection))
                {
                    newOrderID = (int)selectCommand.ExecuteScalar();
                }
            }

            return newOrderID;
        }

        // ✅ Private helper to map SQL rows to Order objects
        private List<Order> MapOrdersFromQuery(string query, SqlParameter[] parameters)
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddRange(parameters);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int orderId = Convert.ToInt32(reader["orderID"]);
                        int employeeId = Convert.ToInt32(reader["employeeID"]);
                        int tableId = Convert.ToInt32(reader["tableID"]);
                        DateTime orderTime = Convert.ToDateTime(reader["order_time"]);
                        bool isServed = Convert.ToBoolean(reader["isServed"]);

                        Employee employee = _employeeRepository.GetEmployeeByID(employeeId);
                        Table table = GetTableById(tableId);

                        orders.Add(new Order(
                            orderID: orderId,
                            employee: employee,
                            table: table,
                            orderTime: orderTime,
                            isServed: isServed,
                            orderItems: new List<OrderItem>() // Add if needed later
                        ));
                    }
                }
            }

            return orders;
        }

        // ✅ Private helper to get full Table object (no lambda)
        private Table GetTableById(int tableId)
        {
            List<Table> tables = _tableRepository.GetAllTables();
            foreach (Table t in tables)
            {
                if (t.TableId == tableId)
                    return t;
            }

            throw new Exception($"Table with ID {tableId} not found.");
        }
    }
}

