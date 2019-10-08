![Nuget](https://img.shields.io/nuget/v/ag.DbData.SQLite)
# ag.DbData.SQLite
A library for working with SQLite databases in .NET Framework, .NET Core and .NET Standard projects.

## Usage
>#### to use ```ag.DbData.SQLite``` in ```.NET Framework``` project, either install ```System.Data.SQLite``` package from [official site](https://system.data.sqlite.org), or add ```x86/x64``` folders with ```SQLite.Interop.dll``` to your output location

1. Add section to settings file (optional)
```csharp
{
  "DbDataSettings": {
    "AllowExceptionLogging": false 
  }
}
```
2. Add appropriate usings:
```csharp
using ag.DbData.SQLite.Extensions;
using ag.DbData.SQLite.Factories;
```
3. Register services with extension method:
```csharp
services.AddAgSQLite();
// or
services.AddAgSQLite(config.GetSection("DbDataSettings"));
// or
services.AddAgSQLite(opts =>
{
  opts.AllowExceptionLogging = false; 
});
```
4. Inject ISQLiteDbDataFactory into your classes:
```csharp
private readonly ISQLiteDbDataFactory _sqLiteFactory;

public MyClass(ISQLiteDbDataFactory sqLiteFactory)
{
    _sqLiteFactory = sqLiteFactory;
}
```
5. Obtain new instance of ```IDbDataObject``` by calling factory's ```Create``` method. *```IDbDataObject``` interface implements ```IDisposable```, so use it into ```using``` directive*:
```csharp
using (var sqLiteDbData = _sqLiteFactory.Create(YOUR_CONNECTION_STRING))
{
    using (var t = sqLiteDbData.FillDataTable("SELECT * FROM YOUR_TABLE"))
    {
        foreach (DataRow r in t.Rows)
        {
            Console.WriteLine(r[0]);
        }
    }
}
```

## Installation
Use Nuget package manager.
- [ag.DbData.SQLite](https://www.nuget.org/packages/ag.DbData.SQLite/)

## Available properties and methods
#### DbDataSettings properties:
```csharp
bool AllowExceptionLogging;
```
Specifies whether exceptions logging is allowed. Default value is ```true```.
#### Extension methods:
```csharp
IServiceCollection AddAgSQLite(this IServiceCollection services)
```
Appends the registration of ```IDbDataFactory``` and ```IDbDataObject``` to ```IServiceCollection```.
```csharp
IServiceCollection AddAgSQLite(this IServiceCollection services, IConfigurationSection configurationSection)
```
Appends the registration of ```IDbDataFactory``` and ```IDbDataObject``` to ```IServiceCollection``` and registers a configuration instance.
```csharp
IServiceCollection AddAgSQLite(this IServiceCollection services, Action<DbDataSettings> configureOptions)
```
Appends the registration of ```IDbDataFactory``` and ```IDbDataObject``` to ```IServiceCollection``` and configures the options.
#### IDbDataFactory methods:
```csharp
IDbDataObject Create(string connectionString)
```
Creates ```IDbDataObject```, specifying database connection string.
#### IDbDataObject methods:
```csharp
DataSet FillDataSet(string query);
```
Fills ```DataSet``` accordingly to specified SQL query. Returns ```DataSet```.
```csharp
DataSet FillDataSet(string query, int timeout);
```
Fills ```DataSet``` accordingly to specified SQL query and command timeout. Returns ```DataSet```.
```csharp
DataSet FillDataSet(string query, IEnumerable<string> tables);
```
Fills ```DataSet``` accordingly to specified SQL query, storing results in tables with names specified in ```tables``` parameter. Returns ```DataSet```.
```csharp
DataSet FillDataSet(string query, IEnumerable<string> tables, int timeout);
```
Fills ```DataSet``` accordingly to specified SQL query and command timeout, storing results in tables with names specified in ```tables``` parameter.  Returns ```DataSet```.
```csharp
DataSet FillDataSetInTransaction(string query);
```
Fills ```DataSet``` in transaction accordingly to specified SQL query. Returns ```DataSet```.
```csharp
DataSet FillDataSetInTransaction(string query, int timeout);
```
Fills ```DataSet``` in transaction accordingly to specified SQL query and command timeout. Returns ```DataSet```.
```csharp
DataSet FillDataSetInTransaction(string query, IEnumerable<string> tables);
```
Fills ```DataSet``` in transaction accordingly to specified SQL query, storing results in specified tables. Returns ```DataSet```.
```csharp
DataSet FillDataSetInTransaction(string query, IEnumerable<string> tables, int timeout);
```
Fills ```DataSet``` in transaction accordingly to specified SQL query and command timeout, storing results in specified tables. Returns ```DataSet```.
```csharp
DataTable FillDataTable(string query);
```
Fills ```DataTable``` accordingly to specified SQL query. Returns ```DataTable```.
```csharp
DataTable FillDataTable(string query, int timeout);
```
Fills ```DataTable``` accordingly to specified SQL query and command timeout. Returns ```DataTable```.
```csharp
DataTable FillDataTableInTransaction(string query);
```
Fills ```DataTable``` in transaction accordingly to specified SQL query. Returns ```DataTable```.
```csharp
DataTable FillDataTableInTransaction(string query, int timeout);
```
Fills ```DataTable``` in transaction accordingly to specified SQL query and command timeout. Returns ```DataTable```.
```csharp
int Execute(string query);
```
Executes specified query. Returns numbers of rows affected by execution.
```csharp
int Execute(string query, int timeout);
```
Executes specified query with specified command timeout. Returns numbers of rows affected by execution.
```csharp
int ExecuteInTransaction(string query);
```
Executes specified query in transaction. Returns numbers of rows affected by execution.
```csharp
int ExecuteInTransaction(string query, int timeout);
```
Executes specified query in transaction with specified command timeout. Returns numbers of rows affected by execution.
```csharp
DbDataReader GetDataReader(string query);
```
Gets ```DbDataReader``` for specified SQL query. Returns ```DataReader```.
```csharp
DbDataReader GetDataReader(string query, int timeout);
```
Gets ```DbDataReader``` for specified SQL query with specified command timeout. Returns ```DataReader```.
```csharp
int ExecuteCommand(DbCommand cmd);
```
Executes ```DbCommand```. Returns number of rows affected by execution.
```csharp
int ExecuteCommand(DbCommand cmd, int timeout);
```
Executes ```DbCommand``` with specified command timeout. Returns number of rows affected by execution.
```csharp
int ExecuteCommandInTransaction(DbCommand cmd);
```
Executes ```DbCommand``` in transaction. Returns number of rows affected by execution.
```csharp
int ExecuteCommandInTransaction(DbCommand cmd, int timeout);
```
Executes ```DbCommand``` in transaction with specified command timeout. Returns number of rows affected by execution.
```csharp
DataTable GetSchema();
```
Gets schema information for the data source of ```DbDataObject``` connection. Returns ```DataTable```.
```csharp
DataTable GetSchema(string collectionName);
```
Gets schema information for the data source of ```DbDataObject``` connection using the specified string for the schema name. Returns ```DataTable```.
```csharp
DataTable GetSchema(string collectionName, string[] restrictedValues);
```
Gets schema information for the data source of ```DbDataObject``` connection using the specified string for the schema name and the specified string array for the restriction values. Returns ```DataTable```.
```csharp
object GetScalar(string query);
```
Gets scalar value for specified SQL query. Returns ```object```.
```csharp
object GetScalar(string query, int timeout);
```
Gets scalar value for specified SQL query and command timeout. Returns ```object```.
```csharp
object GetScalarInTransaction(string query);
```
Gets scalar value for specified SQL query in transaction. Returns ```object```.
```csharp
object GetScalarInTransaction(string query, int timeout);
```
Gets scalar value for specified SQL query and command timeout in transaction. Returns ```object```.
```csharp
bool BeginTransaction(string connectionString);
```
Begins transaction on database specified in connection string. Returns ```true``` if transaction has been started, ```false``` otherwise.
```csharp
void CommitTransaction();
```
Commits transaction.
```csharp
void RollbackTransaction();
```
Rolls back transaction.

## Credits
ag.DbData.SQLite is built with the following projects:
- [System.Data.SQLite.Core](https://system.data.sqlite.org/index.html/doc/trunk/www/index.wiki)
