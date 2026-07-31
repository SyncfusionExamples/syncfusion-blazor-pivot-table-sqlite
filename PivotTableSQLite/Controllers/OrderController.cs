using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor;

namespace PivotTableSQLite.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly string connectionString;

        public OrderController(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("SQLite")
                ?? throw new InvalidOperationException(
                    "The SQLite connection string is not configured.");
        }

        [HttpPost]
        public object Post([FromBody] DataManagerRequest request)
        {
            _ = request;

            List<Order> dataSource = GetOrderData();

            return new
            {
                result = dataSource,
                count = dataSource.Count
            };
        }

        private List<Order> GetOrderData()
        {
            List<Order> orders = new();

            const string query =
                @"SELECT
                    OrderID,
                    CustomerName,
                    EmployeeID,
                    Freight,
                    ShipCity
                  FROM Orders
                  ORDER BY OrderID";

            using SqliteConnection connection = new(connectionString);
            connection.Open();

            using SqliteCommand command = new(query, connection);
            using SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                orders.Add(new Order
                {
                    OrderID = reader.IsDBNull(0) ? null : reader.GetInt32(0),
                    CustomerName = reader.IsDBNull(1) ? null : reader.GetString(1),
                    EmployeeID = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                    Freight = reader.IsDBNull(3) ? null : Convert.ToDecimal(reader.GetDouble(3)),
                    ShipCity = reader.IsDBNull(4) ? null : reader.GetString(4)
                });
            }

            return orders;
        }

        [HttpPost("Insert")]
        public IActionResult Insert([FromBody] CRUDModel<Order> value)
        {
            if (value.Value is not Order order
                || string.IsNullOrWhiteSpace(order.CustomerName)
                || !order.EmployeeID.HasValue)
            {
                return BadRequest(
                    "CustomerName and EmployeeID are required.");
            }

            const string query =
                @"INSERT INTO Orders
                    (CustomerName, EmployeeID, Freight, ShipCity)
                  VALUES
                    (@CustomerName, @EmployeeID, @Freight, @ShipCity)";

            using SqliteConnection connection = new(connectionString);
            connection.Open();

            using SqliteCommand command = new(query, connection);

            command.Parameters.AddWithValue(
                "@CustomerName",
                order.CustomerName);

            command.Parameters.AddWithValue(
                "@EmployeeID",
                order.EmployeeID.Value);

            command.Parameters.AddWithValue(
                "@Freight",
                order.Freight.HasValue
                    ? order.Freight.Value
                    : DBNull.Value);

            command.Parameters.AddWithValue(
                "@ShipCity",
                order.ShipCity ?? (object)DBNull.Value);

            command.ExecuteNonQuery();

            return Ok(order);
        }

        [HttpPost("Update")]
        public IActionResult Update([FromBody] CRUDModel<Order> value)
        {
            if (value.Value is not Order order
                || !order.OrderID.HasValue
                || string.IsNullOrWhiteSpace(order.CustomerName)
                || !order.EmployeeID.HasValue)
            {
                return BadRequest(
                    "OrderID, CustomerName and EmployeeID are required.");
            }

            const string query =
                @"UPDATE Orders
                  SET CustomerName = @CustomerName,
                      EmployeeID = @EmployeeID,
                      Freight = @Freight,
                      ShipCity = @ShipCity
                  WHERE OrderID = @OrderID";

            using SqliteConnection connection = new(connectionString);
            connection.Open();

            using SqliteCommand command = new(query, connection);

            command.Parameters.AddWithValue(
                "@CustomerName",
                order.CustomerName);

            command.Parameters.AddWithValue(
                "@EmployeeID",
                order.EmployeeID.Value);

            command.Parameters.AddWithValue(
                "@Freight",
                order.Freight.HasValue
                    ? order.Freight.Value
                    : DBNull.Value);

            command.Parameters.AddWithValue(
                "@ShipCity",
                order.ShipCity ?? (object)DBNull.Value);

            command.Parameters.AddWithValue(
                "@OrderID",
                order.OrderID.Value);

            int rowsAffected = command.ExecuteNonQuery();

            return rowsAffected == 0
                ? NotFound()
                : Ok(order);
        }

        [HttpPost("Delete")]
        public IActionResult Delete([FromBody] CRUDModel<Order> value)
        {
            if (!int.TryParse(value.Key?.ToString(), out int orderId))
            {
                return BadRequest(
                    "A numeric order key is required.");
            }

            const string query =
                @"DELETE FROM Orders
                  WHERE OrderID = @OrderID";

            using SqliteConnection connection = new(connectionString);
            connection.Open();

            using SqliteCommand command = new(query, connection);

            command.Parameters.AddWithValue(
                "@OrderID",
                orderId);

            int rowsAffected = command.ExecuteNonQuery();

            return rowsAffected == 0
                ? NotFound()
                : NoContent();
        }

        public class Order
        {
            [Key]
            public int? OrderID { get; set; }

            public string? CustomerName { get; set; }

            public int? EmployeeID { get; set; }

            public decimal? Freight { get; set; }

            public string? ShipCity { get; set; }
        }

        public class CRUDModel<T> where T : class
        {
            [JsonPropertyName("action")]
            public string? Action { get; set; }

            [JsonPropertyName("keyColumn")]
            public string? KeyColumn { get; set; }

            [JsonPropertyName("key")]
            public object? Key { get; set; }

            [JsonPropertyName("value")]
            public T? Value { get; set; }

            [JsonPropertyName("added")]
            public List<T>? Added { get; set; }

            [JsonPropertyName("changed")]
            public List<T>? Changed { get; set; }

            [JsonPropertyName("deleted")]
            public List<T>? Deleted { get; set; }

            [JsonPropertyName("params")]
            public IDictionary<string, object>? Params { get; set; }
        }
    }
}