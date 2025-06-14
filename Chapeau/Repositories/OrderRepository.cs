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
        private readonly IOrderItemRepository _orderItemRepository;

        public OrderRepository(IConfiguration configuration, ITableRepository tableRepository, IEmployeeRepository employeeRepository, IOrderItemRepository orderItemRepository)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _tableRepository = tableRepository;
            _employeeRepository = employeeRepository;
            _orderItemRepository = orderItemRepository;
        }

        //take an order
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

        //kitchen or bar
        public List<Order> GetOrdersByStatus(Status status, string type, DateTime createdAfter)
        {
            List<Order> orders = new List<Order>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * " +
                    "FROM [Order] " +
                    "WHERE orderID IN (" +
                    "SELECT orderID FROM OrderItem JOIN MenuItem ON OrderItem.itemID = MenuItem.itemID " +
                    "WHERE item_type = @type AND [status] = @status) AND orderTime > @time " +
                    "ORDER BY orderTime;";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@status", status.ToString());
                command.Parameters.AddWithValue("@type", type.ToString());
                command.Parameters.AddWithValue("@time", createdAfter);
                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Order order = ReadOrder(reader, status, type);
                    orders.Add(order);
                }
                reader.Close();
            }
            return orders;
        }

        private Order ReadOrder(SqlDataReader reader, Status status, string type)
        {
            int orderId = (int)reader["orderID"];

            int employeeID = (int)reader["employeeID"];
            Employee employee = _employeeRepository.GetEmployeeByID(employeeID);

            int tableID = (int)reader["tableID"];
            Table table = _tableRepository.GetTableByID(tableID);

            DateTime orderTime = (DateTime)reader["orderTime"];
            //bool isServed = true;
            List<OrderItem> orderItems = _orderItemRepository.GetOrderItemsByOrderID(orderId, status, type);

            return new Order(orderId, employee, table, orderTime, orderItems);

        }
    }
}

