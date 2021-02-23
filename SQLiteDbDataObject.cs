using ag.DbData.Abstraction;
using ag.DbData.Abstraction.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ag.DbData.SQLite
{
    /// <summary>
    /// Represents SQLiteDbDataObject object.
    /// </summary>
    internal class SQLiteDbDataObject : DbDataObject
    {
        #region ctor

        /// <summary>
        /// Creates new instance of <see cref="SQLiteDbDataObject"/>.
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> object.</param>
        /// <param name="options"><see cref="DbDataSettings"/> options.</param>
        /// <param name="stringProvider"><see cref="IDbDataStringProvider"/> object.</param>
        public SQLiteDbDataObject(ILogger<IDbDataObject> logger, IOptions<SQLiteDbDataSettings> options,
            IDbDataStringProvider stringProvider) :
            base(logger, options, stringProvider)
        {
            var connectionString = StringProvider.ConnectionString;
            if (!string.IsNullOrEmpty(connectionString))
                Connection = new SQLiteConnection(connectionString);
        }
        #endregion

        #region Overrides
        /// <inheritdoc />
        public override DataSet FillDataSet(string query) => innerFillDataSet(query, null, -1, false);

        /// <inheritdoc />
        public override DataSet FillDataSet(string query, int timeout) => innerFillDataSet(query, null, timeout, false);

        /// <inheritdoc />
        public override DataSet FillDataSet(string query, IEnumerable<string> tables) => innerFillDataSet(query, tables, -1, false);

        /// <inheritdoc />
        public override DataSet FillDataSet(string query, IEnumerable<string> tables, int timeout) => innerFillDataSet(query, tables, timeout, false);

        /// <inheritdoc />
        public override DataSet FillDataSetInTransaction(string query) => innerFillDataSet(query, null, -1, true);

        /// <inheritdoc />
        public override DataSet FillDataSetInTransaction(string query, int timeout) => innerFillDataSet(query, null, timeout, true);

        /// <inheritdoc />
        public override DataSet FillDataSetInTransaction(string query, IEnumerable<string> tables) => innerFillDataSet(query, tables, -1, true);

        /// <inheritdoc />
        public override DataSet FillDataSetInTransaction(string query, IEnumerable<string> tables, int timeout) => innerFillDataSet(query, tables, timeout, true);

        /// <inheritdoc />
        public override DataTable FillDataTable(string query) => innerFillDataTable(query, -1, false);

        /// <inheritdoc />
        public override DataTable FillDataTable(string query, int timeout) => innerFillDataTable(query, timeout, false);

        /// <inheritdoc />
        public override DataTable FillDataTableInTransaction(string query) =>
            innerFillDataTable(query, -1, true);

        /// <inheritdoc />
        public override DataTable FillDataTableInTransaction(string query, int timeout) =>
            innerFillDataTable(query, timeout, true);

        /// <inheritdoc />
        public override DataTable FillDataTable(DbCommand dbCommand) => innerFillDataTable((SQLiteCommand)dbCommand, -1, false);

        /// <inheritdoc />
        public override DataTable FillDataTable(DbCommand dbCommand, int timeout) => innerFillDataTable((SQLiteCommand)dbCommand, timeout, false);

        /// <inheritdoc />
        public override DataTable FillDataTableInTransaction(DbCommand dbCommand) => innerFillDataTable((SQLiteCommand)dbCommand, -1, true);

        /// <inheritdoc />
        public override DataTable FillDataTableInTransaction(DbCommand dbCommand, int timeout) => innerFillDataTable((SQLiteCommand)dbCommand, timeout, true);

        /// <inheritdoc />
        public override int ExecuteCommand(DbCommand cmd) => innerExecuteCommand((SQLiteCommand)cmd, -1, false);

        /// <inheritdoc />
        public override int ExecuteCommand(DbCommand cmd, int timeout) => innerExecuteCommand((SQLiteCommand)cmd, timeout, false);

        /// <inheritdoc />
        public override int ExecuteCommandInTransaction(DbCommand cmd) =>
            innerExecuteCommand((SQLiteCommand)cmd, -1, true);

        /// <inheritdoc />
        public override int ExecuteCommandInTransaction(DbCommand cmd, int timeout) =>
            innerExecuteCommand((SQLiteCommand)cmd, timeout, true);

        /// <inheritdoc />
        public override bool BeginTransaction(string connectionString)
        {
            return innerBeginTransaction(connectionString);
        }

        /// <inheritdoc />
        public override bool BeginTransaction()
        {
            return innerBeginTransaction(StringProvider.ConnectionString);
        }

        /// <inheritdoc />
        public override async Task<int> ExecuteAsync(string query) => await innerExecuteAsync(query, CancellationToken.None);

        /// <inheritdoc />
        public override async Task<int> ExecuteAsync(string query, int timeout) =>
            await innerExecuteAsync(query, CancellationToken.None, timeout);

        /// <inheritdoc />
        public override async Task<int> ExecuteAsync(string query, CancellationToken cancellationToken) =>
            await innerExecuteAsync(query, cancellationToken);

        /// <inheritdoc />
        public override async Task<int> ExecuteAsync(string query, int timeout, CancellationToken cancellationToken) =>
            await innerExecuteAsync(query, cancellationToken, timeout);

        /// <inheritdoc />
        public override async Task<object> GetScalarAsync(string query) => await innerGetScalarAsync(query, CancellationToken.None);

        /// <inheritdoc />
        public override async Task<object> GetScalarAsync(string query, int timeout) => await
            innerGetScalarAsync(query, CancellationToken.None, timeout);

        /// <inheritdoc />
        public override async Task<object> GetScalarAsync(string query, CancellationToken cancellationToken) => await
            innerGetScalarAsync(query, cancellationToken);

        /// <inheritdoc />
        public override async Task<object> GetScalarAsync(string query, int timeout,
            CancellationToken cancellationToken) => await innerGetScalarAsync(query, cancellationToken, timeout);

        #endregion

        #region Private procedures
        private bool innerBeginTransaction(string connectionString)
        {
            try
            {
                if (string.IsNullOrEmpty(connectionString))
                    return false;
                if (TransConnection == null)
                {
                    TransConnection = new SQLiteConnection(connectionString);
                }

                if (TransConnection == null)
                {
                    return false;
                }
                if (TransConnection.State != ConnectionState.Open)
                    TransConnection.Open();
                Transaction = TransConnection.BeginTransaction();
                return true;
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error at BeginTransaction");
                throw new DbDataException(ex, "");
            }
        }

        private DataSet innerFillDataSet(string query, IEnumerable<string> tables, int timeout, bool inTransaction)
        {
            try
            {
                var dataSet = new DataSet();
                using (var cmd = new SQLiteCommand(query, inTransaction
                    ? (SQLiteConnection)TransConnection
                    : (SQLiteConnection)Connection))
                {
                    if (!IsValidTimeout(cmd, timeout))
                        throw new ArgumentException("Invalid CommandTimeout value", nameof(timeout));

                    if (inTransaction)
                        cmd.Transaction = (SQLiteTransaction)Transaction;
                    using (var da = new SQLiteDataAdapter(cmd))
                    {
                        da.Fill(dataSet);
                        if (tables == null) return dataSet;
                        var tablesArray = tables.ToArray();
                        if (tablesArray.Length <= dataSet.Tables.Count)
                            for (var i = 0; i < tablesArray.Length; i++)
                                dataSet.Tables[i].TableName = tablesArray[i];
                        else
                            for (var i = 0; i < dataSet.Tables.Count; i++)
                                dataSet.Tables[i].TableName = tablesArray[i];
                    }
                }
                return dataSet;
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, $"Error at FillDataSet; command text: {query}");
                throw new DbDataException(ex, query);
            }
        }

        private DataTable innerFillDataTable(string query, int timeout, bool inTransaction)
        {
            try
            {
                var table = new DataTable();
                using (var cmd = new SQLiteCommand(query, inTransaction
                    ? (SQLiteConnection)TransConnection
                    : (SQLiteConnection)Connection))
                {
                    if (!IsValidTimeout(cmd, timeout))
                        throw new ArgumentException("Invalid CommandTimeout value", nameof(timeout));

                    if (inTransaction)
                        cmd.Transaction = (SQLiteTransaction)Transaction;
                    using (var da = new SQLiteDataAdapter(cmd))
                    {
                        da.Fill(table);
                    }
                }
                return table;
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, $"Error at FillDataTable; command text: {query}");
                throw new DbDataException(ex, query);
            }
        }

        private DataTable innerFillDataTable(SQLiteCommand command, int timeout, bool inTransaction)
        {
            try
            {
                var table = new DataTable();
                command.Connection = inTransaction
                    ? (SQLiteConnection)TransConnection
                    : (SQLiteConnection)Connection;
                if (!IsValidTimeout(command, timeout))
                    throw new ArgumentException("Invalid CommandTimeout value", nameof(timeout));

                if (inTransaction)
                    command.Transaction = (SQLiteTransaction)Transaction;
                using (var da = new SQLiteDataAdapter(command))
                {
                    da.Fill(table);
                }
                return table;
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, $"Error at FillDataTable; command text: {command.CommandText}");
                throw new DbDataException(ex, command.CommandText);
            }
        }

        private int innerExecuteCommand(SQLiteCommand cmd, int timeout, bool inTransaction)
        {
            try
            {
                if (!IsValidTimeout(cmd, timeout))
                    throw new ArgumentException("Invalid CommandTimeout value", nameof(timeout));

                if (inTransaction)
                {
                    cmd.Connection = (SQLiteConnection)TransConnection;
                    cmd.Transaction = (SQLiteTransaction)Transaction;
                }
                else
                {
                    cmd.Connection = (SQLiteConnection)Connection;
                    Connection.Open();
                }
                var rows = cmd.ExecuteNonQuery();
                return rows;
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, $"Error at ExecuteCommand; command text: {cmd.CommandText}");
                throw new DbDataException(ex, cmd.CommandText);
            }
            finally
            {
                if (!inTransaction)
                    if (Connection.State == ConnectionState.Open)
                        Connection.Close();
            }
        }

        private async Task<int> innerExecuteAsync(string query, CancellationToken cancellationToken, int timeout = -1)
        {
            try
            {
                return await Task.Run(async () =>
                {
                    int rows;
                    using (var asyncConnection = new SQLiteConnection(StringProvider.ConnectionString))
                    {
                        using (var cmd = asyncConnection.CreateCommand())
                        {
                            cmd.CommandText = query;
                            if (!IsValidTimeout(cmd, timeout))
                                throw new ArgumentException("Invalid CommandTimeout value", nameof(timeout));

                            await asyncConnection.OpenAsync(cancellationToken);

                            rows = await cmd.ExecuteNonQueryAsync(cancellationToken);
                        }
                    }
                    return rows;
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, $"Error at ExecuteCommandAsync; command text: {query}");
                throw new DbDataException(ex, query);
            }
        }

        private async Task<object> innerGetScalarAsync(string query, CancellationToken cancellationToken, int timeout = -1)
        {
            try
            {
                return await Task.Run(async () =>
                {
                    object obj;
                    using (var asyncConnection = new SQLiteConnection(StringProvider.ConnectionString))
                    {
                        using (var cmd = asyncConnection.CreateCommand())
                        {
                            cmd.CommandText = query;
                            if (!IsValidTimeout(cmd, timeout))
                                throw new ArgumentException("Invalid CommandTimeout value", nameof(timeout));

                            await asyncConnection.OpenAsync(cancellationToken);

                            obj = await cmd.ExecuteScalarAsync(cancellationToken);
                        }
                    }
                    return obj;
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, $"Error at GetScalarAsync; command text: {query}");
                throw new DbDataException(ex, query);
            }
        }
        #endregion
    }
}
