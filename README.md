# Blazor Pivot Table – SQLite Database Connection using ASP.NET Core

## Project Overview

This repository demonstrates how to bind SQLite data to the Syncfusion [Blazor Pivot Table](https://www.syncfusion.com/blazor-components/blazor-pivot-table) through an ASP.NET Core API and the URL Adaptor. The sample uses the Microsoft.Data.Sqlite provider to load and persist records from an Orders table, while the Pivot Table consumes the API through SfDataManager.

The implementation follows the current guide-based pattern where the UI requests data from the server, and the server handles SQLite access, validation, and CRUD operations. This keeps the component focused on aggregation and editing while the API manages persistence.

### Key Features

- SQLite-backed data source for a lightweight local sample
- Syncfusion Blazor Pivot Table with URL Adaptor remote binding
- CRUD operations exposed through API endpoints
- Same-origin relative URLs for read and write requests
- Primary key configuration for drill-through editing
- Problem details middleware and exception handling for API failures

## Repository

Sample location: https://github.com/SyncfusionExamples/syncfusion-blazor-pivot-table-sqlite-database-binding-sample

## Prerequisites

| Component | Version | Purpose |
|-----------|---------|---------|
| .NET SDK | 10.0 | Build and run the sample targeting net10.0 |
| Visual Studio 2026 / VS Code | 18.0+ / latest | Development environment with ASP.NET and web workloads |
| SQLite | 3.x or later | Database engine and tooling |
| Microsoft.Data.Sqlite | 10.0.10 | SQLite provider for .NET |
| Syncfusion.Blazor.PivotTable | 34.1.32 | Pivot Table UI component |
| Syncfusion.Blazor.Themes | 34.1.32 | Pivot Table styling |
| Newtonsoft.Json | 13.0.3 | JSON serialization for CRUD payloads |

## SQLite Setup

### 1. Create the database file

Create or point to an SQLite database file that will store the Orders table.

### 2. Create the table

Use the following SQL to create the Orders table used by the sample:

```sql
CREATE TABLE IF NOT EXISTS Orders
(
    OrderID      INTEGER PRIMARY KEY AUTOINCREMENT,
    CustomerName TEXT    NOT NULL,
    EmployeeID   INTEGER NOT NULL,
    ShipCity     TEXT    NULL,
    Freight      REAL    NULL
);
```

### 3. Insert sample data (optional)

```sql
INSERT INTO Orders (CustomerName, EmployeeID, ShipCity, Freight) VALUES
('Toms',   1, 'New York',  35.30),
('Ravi',   2, 'London',    80.20),
('Sven',   1, 'Berlin',    52.10),
('Sara',   3, 'Madrid',    18.40),
('Paul',   2, 'Tokyo',     64.75);
```

### 4. Configure the connection string

The application reads the SQLite connection string from appsettings.json:

```json
{
  "ConnectionStrings": {
    "SQLite": "Data Source=<DATABASE_FILE_NAME>"
  }
}
```

Replace the placeholder with the actual database file name or a full absolute path such as `Data Source=C:/Data/Orders.db`.

## Project Structure

| File/Folder | Purpose |
|-------------|---------|
| /Controllers/OrderController.cs | API controller exposing read and CRUD endpoints using SQLite commands |
| /Components/Pages/Home.razor | Pivot Table page with URL Adaptor configuration, editing, and drill-through setup |
| /Program.cs | ASP.NET Core and Syncfusion service registration, middleware, and endpoint mapping |
| /appsettings.json | SQLite connection string configuration |
| /PivotTableSQLite.csproj | Project file with Syncfusion packages and SQLite NuGet references |

## Application Flow

```text
Blazor Pivot Table
        ↓
ASP.NET Core Controller
        ↓
SQLite Connection
        ↓
SQLite Database
```

1. The Pivot Table sends a request through SfDataManager.
2. The URL Adaptor posts the request to the API endpoint.
3. The controller resolves the request through SQLite commands.
4. The response is returned to the client in the format expected by the Pivot Table.

## Updated Implementation Notes

### Program.cs

The current sample uses the following ASP.NET Core configuration:

```csharp
builder.Services.AddSyncfusionBlazor();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddControllers();
builder.Services.AddProblemDetails();

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.MapControllers();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
```

### Home.razor

The Pivot Table uses same-origin relative URLs and marks OrderID as the primary key for drill-through editing:

```razor
<SfDataManager Url="/api/Order"
               InsertUrl="/api/Order/Insert"
               UpdateUrl="/api/Order/Update"
               RemoveUrl="/api/Order/Delete"
               Adaptor="Adaptors.UrlAdaptor">
</SfDataManager>
```

```csharp
private void BeginDrillThrough(BeginDrillThroughEventArgs args)
{
    for (int i = 0; i < args.GridObj.Columns.Count; i++)
    {
        if (args.GridObj.Columns[i].Field == "OrderID")
        {
            args.GridObj.Columns[i].IsPrimaryKey = true;
        }
        else
        {
            args.GridObj.Columns[i].Visible = true;
        }
    }
}
```

## CRUD Operations

The sample supports read, insert, update, and delete operations through the URL Adaptor.

| Endpoint | Method | Payload | Description |
|----------|--------|---------|-------------|
| /api/Order | POST | DataManagerRequest | Returns `{ result, count }` for Pivot Table binding |
| /api/Order/Insert | POST | CRUDModel<Order> | Inserts a new order and returns the created row |
| /api/Order/Update | POST | CRUDModel<Order> | Updates an existing order |
| /api/Order/Delete | POST | CRUDModel<Order> | Deletes an order using the key value |

## Data Model

The Order model used by the controller and Razor page is:

```csharp
public class Order
{
    public int? OrderID { get; set; }
    public string? CustomerName { get; set; }
    public int? EmployeeID { get; set; }
    public decimal? Freight { get; set; }
    public string? ShipCity { get; set; }
}
```

## Getting Started

1. Clone or open the repository in Visual Studio or VS Code.
2. Restore packages and build:

   ```bash
   dotnet restore
   dotnet build
   ```

3. Create the SQLite database file and Orders table as described above.
4. Run the application:

   ```bash
   dotnet run --project PivotTableSQLite
   ```

5. Open the local URL shown in the terminal and verify that the Pivot Table loads and the CRUD endpoints respond correctly.

## Full Documentation

For the complete step-by-step guide, including URL Adaptor configuration, CRUD mappings, and SQLite connection setup, see the official Syncfusion Blazor Pivot Table documentation: https://help.syncfusion.com/grid-sdk/blazor/pivot-table/connecting-to-data-source/sqlite-server

## Support and Feedback

- For general product questions, visit the [Syncfusion Community Forum](https://www.syncfusion.com/forums) or [Syncfusion Support](https://www.syncfusion.com/support).
- To report an issue specific to this sample, open a GitHub issue in this repository.

## License

This is a Syncfusion sample project provided to demonstrate product usage. Review the [Syncfusion license terms](https://www.syncfusion.com/sales/pricing?category=ui-components) before using Syncfusion components in your own applications.
