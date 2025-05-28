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
        //Insert + return full Order
        public Order TakeNewOrder(int tableId, Employee employee)
        {
            int newOrderID = InsertNewOrder(tableId, employee);
            return GetOrderByID(newOrderID);
        }

        //Get Order by ID
        public Order GetOrderByID(int orderID)
        {
            string query = "SELECT * FROM [Order] WHERE orderID = @orderID";
            SqlParameter[] parameters = {
                new SqlParameter("@orderID", orderID)
            };

            return MapOrdersFromQuery(query, parameters).FirstOrDefault();
        }

        //Private helper to insert only
        private int InsertNewOrder(int tableId, Employee employee)
        {
            int newOrderID;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
            INSERT INTO [Order] (employeeID, tableID, orderTime)
            VALUES (@employeeID, @tableID, @orderTime);
            SELECT CAST(SCOPE_IDENTITY() AS int);";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@employeeID", employee.employeeID);
                    command.Parameters.AddWithValue("@tableID", tableId);
                    command.Parameters.AddWithValue("@orderTime", DateTime.Now);

                    object result = command.ExecuteScalar();

                    if (result == null || result == DBNull.Value)
                    {
                        throw new Exception("Failed to retrieve new Order ID.");
                    }

                    newOrderID = Convert.ToInt32(result);
                }
            }

            return newOrderID;
        }

        // Private helper to map SQL rows to Order objects
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
                        DateTime orderTime = Convert.ToDateTime(reader["ordertime"]);
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

        //Private helper to get full Table object 
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

