# pixel-celebrate

## Project setup
```
npm install
```

### Compiles and hot-reloads for development
```
npm run serve
```

### Compiles and minifies for production
```
npm run build
```

### Lints and fixes files
```
npm run lint
```

### Customize configuration
See [Configuration Reference](https://cli.vuejs.org/config/).


## SQL Server with LocalDB

This project uses **SQL Server** with **LocalDB** to manage the database. LocalDB is a lightweight version of SQL Server designed for development purposes, offering an easy-to-use database solution without the need for a full SQL Server installation.

### Prerequisites

- **SQL Server LocalDB**: Ensure that SQL Server Express LocalDB is installed on your system. You can download and install it from [Microsoft's official website](https://docs.microsoft.com/en-us/sql/ssdt/download-sql-server-data-tools?view=sql-server-ver15).
  
- **SQL Server Management Studio (SSMS)** (optional but recommended): You can use SSMS to manage your LocalDB instance and interact with the database more easily. Download it [here](https://aka.ms/ssmsfullsetup).

### Setting Up LocalDB

1. **Starting LocalDB**:
   LocalDB is typically started automatically when you connect to it. If not, you can start it manually using the following command in the command prompt or terminal:

   ```bash
   sqllocaldb start


2. **Connecting to LocalDB**:
   To connect to the LocalDB instance, use the following connection string in your project’s configuration or connection settings:

   ```plaintext
   Server=(localdb)\\MSSQLLocalDB;Database=YourDatabaseName;Trusted_Connection=True;

# Entity Framework Commands for Database Management

This section outlines the essential Entity Framework Core commands for managing database migrations in the PixelCelebrate project.

## 1. Add a New Migration

To create a new migration for any changes to the database schema, use the following command:

```bash
dotnet ef migrations add MigrationName
```

## 2. Apply Pending Migrations

```bash
dotnet ef database update
```


