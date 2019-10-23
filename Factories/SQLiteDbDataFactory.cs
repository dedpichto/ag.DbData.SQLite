using ag.DbData.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data.SQLite;

namespace ag.DbData.SQLite.Factories
{
    /// <summary>
    /// Represents SqlDbDataFactory object.
    /// </summary>
    internal class SQLiteDbDataFactory : ISQLiteDbDataFactory
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Creates object of type <see cref="SQLiteDbDataObject"/>.
        /// </summary>
        /// <returns><see cref="SQLiteDbDataObject"/> implementation of <see cref="IDbDataObject"/> interface.</returns>
        public IDbDataObject Create()
        {
            var dbObject = _serviceProvider.GetService<SQLiteDbDataObject>();
            return dbObject;
        }

        /// <summary>
        /// Creates object of type <see cref="SQLiteDbDataObject"/>.
        /// </summary>
        /// <param name="connectionString">Database connection string.</param>
        /// <returns><see cref="SQLiteDbDataObject"/> implementation of <see cref="IDbDataObject"/> interface.</returns>
        public IDbDataObject Create(string connectionString)
        {
            var dbObject = _serviceProvider.GetService<SQLiteDbDataObject>();
            dbObject.Connection = new SQLiteConnection(connectionString);
            return dbObject;
        }
        
        /// <summary>
        /// Creates new SQLiteDbDataFactory object.
        /// </summary>
        /// <param name="serviceProvider"><see cref="IServiceProvider"/></param>
        public SQLiteDbDataFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
    }
}
