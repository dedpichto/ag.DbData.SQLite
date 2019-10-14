using ag.DbData.Abstraction.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ag.DbData.SQLite.Services
{
    /// <summary>
    /// Represents <see cref="SQLiteStringProviderFactory"/> object.
    /// </summary>
    public class SQLiteStringProviderFactory : IDbDataStringProviderFactory<SQLiteStringProvider>
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Creates new instance of <see cref="SQLiteStringProviderFactory"/>.
        /// </summary>
        /// <param name="serviceProvider"><see cref="IServiceProvider"/>.</param>
        public SQLiteStringProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Creates object of type <see cref="SQLiteStringProvider"/>.
        /// </summary>
        /// <returns>Object of type <see cref="SQLiteStringProvider"/>.</returns>
        public SQLiteStringProvider Get()
        {
            return _serviceProvider.GetService<SQLiteStringProvider>();
        }
    }
}
