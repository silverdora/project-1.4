using Microsoft.AspNetCore.Mvc;
using project-1.4.Models;
using System.Data.SqlClient;

public class MenuController : Controller
{
    private readonly string _connectionString;

    public MenuController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public IActionResult Index()
    {
        List<MenuItem> menuItems = new List<MenuItem>();

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM MenuItem", conn);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                menuItems.Add(new MenuItem
                {
                    itemID = (int)reader["itemID"],
                    item_name = reader["item_name"].ToString(),
                    description = reader["description"].ToString(),
                    price = (decimal)reader["price"],
                    VATPercent = (decimal)reader["VATPercent"],
                    category = reader["category"].ToString(),
                    stockQuantity = (int)reader["stockQuantity"],
                    cardID = (int)reader["cardID"],
                    item_type = reader["item_type"].ToString(),
                    IsActive = (bool)reader["IsActive"]
                });
            }

            reader.Close();
        }

        return View(menuItems);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public IActionResult Create(MenuItem item)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand(@"
                INSERT INTO MenuItem (item_name, description, price, VATPercent, category, stockQuantity, cardID, item_type, IsActive)
                VALUES (@item_name, @description, @price, @VATPercent, @category, @stockQuantity, @cardID, @item_type, 1)", conn);

            cmd.Parameters.AddWithValue("@item_name", item.item_name);
            cmd.Parameters.AddWithValue("@description", item.description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@price", item.price);
            cmd.Parameters.AddWithValue("@VATPercent", item.VATPercent);
            cmd.Parameters.AddWithValue("@category", item.category ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@stockQuantity", item.stockQuantity);
            cmd.Parameters.AddWithValue("@cardID", item.cardID);
            cmd.Parameters.AddWithValue("@item_type", item.item_type ?? (object)DBNull.Value);

            cmd.ExecuteNonQuery();
        }

        return RedirectToAction("Index");
    }

    public IActionResult Edit(int id)
    {
        MenuItem item = new MenuItem();

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM MenuItem WHERE itemID = @itemID", conn);
            cmd.Parameters.AddWithValue("@itemID", id);
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                item.itemID = (int)reader["itemID"];
                item.item_name = reader["item_name"].ToString();
                item.description = reader["description"].ToString();
                item.price = (decimal)reader["price"];
                item.VATPercent = (decimal)reader["VATPercent"];
                item.category = reader["category"].ToString();
                item.stockQuantity = (int)reader["stockQuantity"];
                item.cardID = (int)reader["cardID"];
                item.item_type = reader["item_type"].ToString();
                item.IsActive = (bool)reader["IsActive"]; // Make sure IsActive is included
            }

            reader.Close();
        }

        return View(item);
    }

    [HttpPost]
    public IActionResult Edit(MenuItem item)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand(@"
                UPDATE MenuItem 
                SET item_name = @item_name, description = @description, price = @price, VATPercent = @VATPercent,
                    category = @category, stockQuantity = @stockQuantity, cardID = @cardID, item_type = @item_type
                WHERE itemID = @itemID", conn);

            cmd.Parameters.AddWithValue("@item_name", item.item_name);
            cmd.Parameters.AddWithValue("@description", item.description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@price", item.price);
            cmd.Parameters.AddWithValue("@VATPercent", item.VATPercent);
            cmd.Parameters.AddWithValue("@category", item.category ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@stockQuantity", item.stockQuantity);
            cmd.Parameters.AddWithValue("@cardID", item.cardID);
            cmd.Parameters.AddWithValue("@item_type", item.item_type ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@itemID", item.itemID);

            cmd.ExecuteNonQuery();
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Deactivate(int id)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("UPDATE MenuItem SET IsActive = 0 WHERE itemID = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Activate(int id)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("UPDATE MenuItem SET IsActive = 1 WHERE itemID = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        return RedirectToAction("Index");
    }
}
