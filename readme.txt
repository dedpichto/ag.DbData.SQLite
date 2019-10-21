***************************************************************************************************

ATTENTION! TO USE THIS PACKAGE WITH .NET FRAMEWORK EITHER INSTALL SYSTEM.DATA.SQLITE PACKAGE FROM 
https://system.data.sqlite.org INTO YOUR PROJECT, OR ADD x86/x64 FOLDERS WITH SQLite.Interop.dll 
TO YOUR OUTPUT LOCATION

***************************************************************************************************

// add section to settings file (optional)
{
  "SQLiteDbDataSettings": {
    "AllowExceptionLogging": false, // optional, default is "true"
    "ConnectionString": "YOUR_CONNECTION_STRING" // optional 
  }
}

***************************************************************************************************

// add appropriate usings
using ag.DbData.SQLite.Core.Extensions;
using ag.DbData.SQLite.Core.Factories;

***************************************************************************************************

// register services with extension method

		// ...
		services.AddAgSQLite();
		// or
		services.AddAgSQLite(config.GetSection("SQLiteDbDataSettings"));
		// or
		services.AddAgSQLite(opts =>
        {
            opts.AllowExceptionLogging = false; // optional
			opts.ConnectionString = YOUR_CONNECTION_STRING; // optional  
        });

***************************************************************************************************

// inject ISQLiteDbDataFactory into your classes

        private readonly ISQLiteDbDataFactory _sqLiteFactory;

        public MyClass(ISQLiteDbDataFactory sqLiteFactory)
        {
            _SQLiteFactory = sqLiteFactory;
        }

***************************************************************************************************

// SqLiteDbDataObject implements IDisposable interface, so use it into 'using' directive

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

// in case you have defined connection string in configuration setting you may call Create() method
// without parameter

        using (var sqLiteDbData = _sqLiteFactory.Create())
        {
            using (var t = sqLiteDbData.FillDataTable("SELECT * FROM YOUR_TABLE"))
            {
                foreach (DataRow r in t.Rows)
                {
                    Console.WriteLine(r[0]);
                }
            }
        }

***************************************************************************************************