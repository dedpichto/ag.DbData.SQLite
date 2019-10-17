using ag.DbData.Abstraction;
using ag.DbData.Abstraction.Services;
using ag.DbData.SQLite.Factories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ag.DbData.SQLite.Extensions
{
    /// <summary>
    /// Represents <see cref="ag.DbData.SQLite"/> extensions.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Appends the registration of <see cref="SQLiteDbDataFactory"/> and <see cref="SQLiteDbDataObject"/> services to <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddAgSQLite(this IServiceCollection services)
        {
            services.TryAddTransient<IDbDataStringProvider, DbDataStringProvider>();
            services.AddSingleton<ISQLiteDbDataFactory, SQLiteDbDataFactory>();
            services.AddTransient<SQLiteDbDataObject>();
            return services;
        }

        /// <summary>
        /// Appends the registration of <see cref="SQLiteDbDataFactory"/> and <see cref="SQLiteDbDataObject"/> services to <see cref="IServiceCollection"/> and registers a configuration instance.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="configurationSection">The <see cref="IConfigurationSection"/> being bound.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddAgSQLite(this IServiceCollection services, IConfigurationSection configurationSection)
        {
            services.AddAgSQLite();
            services.Configure<DbDataSettings>(configurationSection);
            return services;
        }

        /// <summary>
        /// Appends the registration of <see cref="SQLiteDbDataFactory"/> and <see cref="SQLiteDbDataObject"/> services to <see cref="IServiceCollection"/> and configures the options.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="configureOptions">The action used to configure the options.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddAgSQLite(this IServiceCollection services,
            Action<DbDataSettings> configureOptions)
        {
            services.AddAgSQLite();
            services.Configure(configureOptions);
            return services;
        }
    }
}
