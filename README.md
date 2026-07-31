# Blazor Pivot Table – SQLite Database Connection using ASP.NET Core

## Project Overview

This repository demonstrates a production-ready pattern for binding an SQLite database to the Syncfusion Blazor Pivot Table using the URL Adaptor. The sample reads and manages order records stored in a SQLite table through an ASP.NET Core application and the Microsoft.Data.Sqlite provider.

The implementation follows a clean data flow where the Pivot Table interacts with a web API endpoint, and the API uses SQLite commands to read and update data. This approach keeps the UI focused on analysis while the server handles data access and persistence.

### Key Features

- SQLite integration for lightweight local data storage
- Syncfusion Blazor Pivot Table support for aggregation and drill-through
- URL Adaptor-based remote data binding
- CRUD operations through the API layer
- Primary key configuration for drill-through editing
- Configurable development port through launch settings

## Repository

Sample Location : https://github.com/SyncfusionExamples/syncfusion-blazor-pivot-table-sqlite-database-binding-sample

## Prerequisites

| Component | Version | Purpose |
|-----------|---------|---------|
| Visual Studio 2022 / VS Code | Latest | Development IDE with Blazor workload |
| .NET SDK | net10.0 or compatible | Runtime and build tools |
| SQLite | 3.x or later | Local relational database engine |
| Microsoft.Data.Sqlite | Latest | SQLite provider for .NET |
| Syncfusion.Blazor.PivotTable | Latest | Pivot Table UI component |
| Syncfusion.Blazor.Themes | Latest | Styling for the Pivot Table component |
| Syncfusion.Blazor.Data | Latest | Data adaptors including URL Adaptor support |
| Newtonsoft.Json | Latest | JSON serialization for CRUD models |

## SQLite Setup

### 1. Create the Database File

Create or point to an SQLite database file that will store the Orders table.

### 2. Create the Table

Use the following SQL to create the Orders table used by the sample:

```sql
CREATE TABLE IF NOT EXISTS Orders
(
    OrderID        INTEGER PRIMARY KEY AUTOINCREMENT,
    CustomerName   TEXT NULL,
    EmployeeID     INTEGER NULL,
    ShipCity       TEXT NULL,
    Freight        REAL NULL
);
```

### 3. Insert Sample Data (Optional)

Seed the table with a few rows so the Pivot Table has data to render on first load:

```sql
INSERT INTO Orders (CustomerName, EmployeeID, ShipCity, Freight) VALUES
('Toms',   1, 'New York',  35.30),
('Ravi',   2, 'London',    80.20),
('Sven',   1, 'Berlin',    52.10),
('Sara',   3, 'Madrid',    18.40),
('Paul',   2, 'Tokyo',     64.75);
```

### 4. Configure the Connection String

The connection string is defined in appsettings.json under the SQLite key:

```json
{
  "ConnectionStrings": {
    "SQLite": "Data Source=<DATABASE_FILE_NAME>"
  }
}
```

Update the value to point to your SQLite database file.

## Project Structure

| File/Folder | Purpose |
|-------------|---------|
| /Controllers/OrderController.cs | REST API controller exposing read and CRUD endpoints using SQLite commands |
| /Components/Pages/Home.razor | Pivot Table page with URL Adaptor configuration, editing, and drill-through setup |
| /Program.cs | Service registration for ASP.NET Core and Syncfusion Blazor |
| /appsettings.json | SQLite connection string configuration |
| /PivotTableSQLite.csproj | Project file with Syncfusion Blazor packages and SQLite NuGet references |

## Application Flow

The sample follows a straightforward server-driven data flow:

```text
Blazor Pivot Table
        ↓
ASP.NET Core Controller
        ↓
SQLite Connection
        ↓
SQLite Database
```

1. The Pivot Table raises a data request through SfDataManager.
2. SfDataManager posts the request to the URL Adaptor endpoint.
3. The OrderController reads or modifies data through SQLite commands.
4. The response is returned to the client in the format expected by the Pivot Table.

## Configuration Steps

### Connection String Setup

Update the SQLite entry in appsettings.json to match the database file you want to use:

```json
{
  "ConnectionStrings": {
    "SQLite": "Data Source=<DATABASE_FILE_NAME>"
  }
}
```

### Getting Started / Quick Start

1. Clone or open the repository in Visual Studio or VS Code.
2. Restore packages and build.

   ```bash
   dotnet restore
   dotnet build
   ```

3. Create the SQLite database file and Orders table as described above.
4. Run the application.

   ```bash
   dotnet run --project PivotTableSQLite
   ```

5. Open the local URL displayed in the terminal.

## CRUD Operations

The sample supports full CRUD operations through the URL Adaptor. Each operation is handled by a dedicated controller endpoint and persisted by SQLite.

### Read

Purpose: Load the orders collection for the Pivot Table.

Flow: The Pivot Table posts a DataManagerRequest to POST /api/Order. The controller retrieves the records through SQLite.

### Insert

Purpose: Add a new order record.

Flow: The Pivot Table posts a new record to POST /api/Order/Insert. The controller validates the data and inserts it into the SQLite table.

### Update

Purpose: Modify an existing order record.

Flow: The Pivot Table posts updated values to POST /api/Order/Update. The controller locates the record and updates it.

### Delete

Purpose: Remove an existing order record.

Flow: The Pivot Table sends a delete request to POST /api/Order/Delete. The controller removes the record from the SQLite table.

## API Endpoints

| Endpoint | HTTP Method | Request Body | Description |
|----------|-------------|--------------|-------------|
| /api/Order | POST | DataManagerRequest | Returns the Orders collection as { result, count } for Pivot Table binding |
| /api/Order/Insert | POST | CRUDModel<Order> | Inserts a new order record into SQLite |
| /api/Order/Update | POST | CRUDModel<Order> | Updates an existing order record in SQLite |
| /api/Order/Delete | POST | CRUDModel<Order> | Deletes an order record from SQLite using the Key field |

## Data Model

The Order model represents a single row in the SQLite Orders table:

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

| Field | Type | Pivot Role | Description |
|-------|------|-----------|-------------|
| OrderID | int | Primary Key | Unique identifier for the order |
| CustomerName | string | Row | Customer name used as a pivot row field |
| EmployeeID | int | Column | Employee ID used as a pivot column field |
| Freight | decimal | Value | Freight amount used as the pivot value |
| ShipCity | string | Dimension | Ship city used as an additional pivot field |

## Full Documentation

For step-by-step guidance on integrating SQLite with the Syncfusion Blazor Pivot Table, including URL Adaptor configuration, CRUD operations, and database connectivity, see the official Syncfusion Blazor Pivot Table documentation.
