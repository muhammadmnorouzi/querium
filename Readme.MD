# Quantiq Dynamic Table Manager

A powerful ASP.NET Core application for managing SQL table schemas through a dynamic API. This project provides a flexible and secure way to create, update, and manage database tables on the fly without the need for manual SQL scripts or traditional migrations.

---

## 🚀 Getting Started

Follow these steps to set up the project locally and begin development.

### Prerequisites

* **.NET SDK:** Version 9.0 or later.
* **SQL Server:** A local or remote SQL Server instance.
* **Arian.Quantiq.Application:** A .NET Core 9.0 project with your application logic.
* **Git:** For cloning the repository.

### Setup Instructions

1.  **Clone the repository:**
    ```
    git clone [your-repository-url]
    cd Arian.Quantiq
    ```

2.  **Update the database connection string:**
    Open `appsettings.json` and update the `DefaultConnection` string with your SQL Server details.
    ```
    {
      "ConnectionStrings": {
        "DefaultConnection": "Server=localhost;Database=QuantiqDb;Integrated Security=True;"
      }
    }
    ```

3.  **Install project dependencies:**
    The project uses **FluentValidation** for input validation and a custom database compiler. Restore all necessary packages by running:
    ```
    dotnet restore
    ```

4.  **Build the project:**
    ```
    dotnet build
    ```

5.  **Run the application:**
    ```
    dotnet run
    ```
    The application will launch, and the API endpoints will be accessible.

---

## 🗃️ Database Management

This project uses a hybrid approach for database management, combining Entity Framework Core for its internal tables and dynamic SQL for user-generated schemas.

### Initial Migration

The application uses EF Core to manage its own internal tables (e.g., `TableDefinitions`). To apply the initial migration, run the following command from the `Arian.Quantiq.Application` directory:

```console
dotnet ef database update --project Arian.Quantiq.Application

```
This will create the necessary `TableDefinitions` table and other internal schema.

### Dynamic Table Creation and Updates

* **Table creation and updates are handled via API calls**, not EF Core migrations.
* Your API endpoints (e.g., `POST /api/SQLTable/CreateTable`) will use the `IDatabaseCompiler` to dynamically generate and execute SQL.
* The `TableDefinitions` table is used to track and validate user-created schemas.

---

## ✅ Current Features

* **Dynamic Table Creation:** Users can create new SQL tables via an API request by providing a `CreateTableDTO`.
* **Dynamic Data Manipulation:** The API supports data import and export for dynamic tables, with **Excel** format currently enabled. Support for additional formats is planned.
* **Database-Agnostic Abstraction:** The application uses abstract data types (`String`, `Integer`, `Decimal`) and a dedicated compiler to generate database-specific SQL, ensuring portability.
* **API-First Design:** The project adheres to modern API best practices using `[ApiController]` to enable features like automatic model validation and `ProblemDetails` for errors.
* **Robust Input Validation:**
    * All incoming requests are validated using **FluentValidation**.
    * The `IDatabaseCompiler` performs an additional layer of validation to ensure that table names and column definitions are valid SQL identifiers.
    * The `SqlServerMetadataService` queries the database's `INFORMATION_SCHEMA.COLUMNS` view to validate update requests against the current live schema.

---

## 📝 TODO List

* SQL Injection attacks should be considered, [parametric query generation and execution]
* Implement a complete table update logic that supports `ALTER TABLE` operations (add, alter, drop, rename).
* Implement more advanced validation rules to prevent a user from dropping a column with a foreign key or a primary key.
* Create a front-end UI for a richer user experience, allowing users to visually build and modify tables.
* Implement a simple user authentication and authorization system to secure the API endpoints.
* Expand data format support beyond Excel (e.g., CSV, JSON, etc.).
