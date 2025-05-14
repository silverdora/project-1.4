using System;
using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace Chapeau.Repositories
{
	public class RunningOrdersRepository : IRunningOrdersRepository
	{
        private readonly string _connectionString;

        public RunningOrdersRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public void ChangeOrderStatus(Order order)
        {
            throw new NotImplementedException();
        }

        public List<Order> GetAllRunningOrders()
        {
            List<Order> orders = new List<Order>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * " +
                    "FROM [Order] " +
                    "WHERE orderID IN (" +
                    "SELECT orderID " +
                    "FROM Includes " +
                    "WHERE [status] != 'completed') " +
                    "ORDER BY orderTime;";
                SqlCommand command = new SqlCommand(query, connection);
                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Order order = ReadOrder(reader);
                    orders.Add(order);
                }
                reader.Close();
            }
            return orders;
        }

        public List<Order> GetOrdersByStatus(string status)
        {
            List<Order> orders = new List<Order>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * " +
                    "FROM [Order] " +
                    "WHERE orderID IN (" +
                    "SELECT orderID " +
                    "FROM Includes " +
                    "WHERE [status] = '@status') " +
                    "ORDER BY orderTime;";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@status", status);
                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Order order = ReadOrder(reader);
                    orders.Add(order);
                }
                reader.Close();
            }
            return orders;
        }
        private Order ReadOrder(SqlDataReader reader)
        {
            int orderId = (int)reader["orderID"];
            int employeeID = (int)reader["employeeID"];
            int tableID = (int)reader["tableID"];
            DateTime orderTime = (DateTime)reader["tableID"];
            bool isServed = true;
            List<Includes> includes = new List<Includes>();

            return new Order(orderId, employeeID, tableID, orderTime, isServed, includes);
        }

        private Includes
    }
}

