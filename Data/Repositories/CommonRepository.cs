using Dextor.API.Data.IRepositories;
using Dextor.API.Models.BindingModel;
using Microsoft.Data.SqlClient; // Replaces System.Data.SqlClient
using Microsoft.EntityFrameworkCore; // Required for EF Core extensions
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common; // Required for DbConnection/DbCommand
using System.Text;

namespace Dextor.API.Data.Repositories
{
    public class CommonRepository : GenericRepository<Dictionary<string, object>>, ICommonRepository
    {
        private readonly DatabaseContext _db;
        private readonly POSDBContext _dbpos;
        private readonly OracleDBContext _dbIFS;
        private readonly DWDBContext _dwdb;

        // 1. UPDATED: Contexts are now injected via Dependency Injection
        public CommonRepository(
            DatabaseContext db,
            POSDBContext dbpos,
            OracleDBContext dbIFS,
            DWDBContext dwdb) : base(db)
        {
            _db = db;
            _dbpos = dbpos;
            _dbIFS = dbIFS;
            _dwdb = dwdb;
        }

        public List<Dictionary<string, object>> GetResult(string query)
        {
            var results = new List<Dictionary<string, object>>();
            if (string.IsNullOrWhiteSpace(query)) return results;
            query = query.Trim();

            if (!query.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase)
                && !query.StartsWith("EXEC", StringComparison.OrdinalIgnoreCase)
                && !query.StartsWith("WITH", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Only SELECT, WITH or EXEC query is allowed.");
            }

            // 2. UPDATED: GetDbConnection() is the EF Core equivalent of .Connection
            var connection = _db.Database.GetDbConnection();
            bool shouldCloseConnection = false;

            try
            {
                // 3. UPDATED: SetCommandTimeout for EF Core
                _db.Database.SetCommandTimeout(600);

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                    shouldCloseConnection = true;
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = query;
                    command.CommandTimeout = 600;

                    using (var reader = command.ExecuteReader())
                    {
                        do
                        {
                            if (reader.FieldCount <= 0) continue;

                            while (reader.Read())
                            {
                                var row = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string columnName = reader.GetName(i);
                                    if (string.IsNullOrWhiteSpace(columnName)) columnName = "Column" + i;

                                    if (row.ContainsKey(columnName))
                                    {
                                        string originalName = columnName;
                                        int counter = 1;
                                        while (row.ContainsKey(columnName))
                                        {
                                            columnName = originalName + "_" + counter;
                                            counter++;
                                        }
                                    }

                                    object value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                    row[columnName] = value;
                                }
                                results.Add(row);
                            }
                        }
                        while (reader.NextResult());
                    }
                }
            }
            finally
            {
                if (shouldCloseConnection && connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return results;
        }

        public string GetJsonResult(string query)
        {
            StringBuilder jsonOutput = new StringBuilder();
            var connection = _db.Database.GetDbConnection();

            try
            {
                _db.Database.SetCommandTimeout(600);
                if (connection.State != ConnectionState.Open) connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = 300;
                    command.CommandText = query;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                            {
                                jsonOutput.Append(reader.GetFieldValue<string>(0));
                            }
                        }
                    }
                }
            }
            finally
            {
                if (connection.State == ConnectionState.Open) connection.Close();
            }

            return jsonOutput.ToString();
        }

        public List<Dictionary<string, object>> GetSearchResult(string query)
        {
            var results = new List<Dictionary<string, object>>();
            var connection = _db.Database.GetDbConnection();

            try
            {
                _db.Database.SetCommandTimeout(600);
                if (connection.State != ConnectionState.Open) connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = 300;
                    command.CommandText = query;

                    using (var reader = command.ExecuteReader())
                    {
                        do
                        {
                            while (reader.Read())
                            {
                                var row = new Dictionary<string, object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                }
                                results.Add(row);
                            }
                        }
                        while (reader.NextResult());
                    }
                }
            }
            finally
            {
                if (connection.State == ConnectionState.Open) connection.Close();
            }

            return results;
        }

        public List<Dictionary<string, object>> GetSearchResultData(string table, Dictionary<string, object> conditions)
        {
            var results = new List<Dictionary<string, object>>();
            var conn = _db.Database.GetDbConnection();

            try
            {
                _db.Database.SetCommandTimeout(300);
                if (conn.State != ConnectionState.Open) conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    var whereClauses = new List<string>();

                    foreach (var kvp in conditions)
                    {
                        string paramName = $"@{kvp.Key}";
                        whereClauses.Add($"{kvp.Key} = {paramName}");

                        var param = cmd.CreateParameter();
                        param.ParameterName = paramName;
                        param.Value = kvp.Value ?? DBNull.Value;
                        cmd.Parameters.Add(param);
                    }

                    string whereSql = whereClauses.Count > 0 ? " WHERE " + string.Join(" AND ", whereClauses) : "";

                    cmd.CommandText = $"SELECT * FROM {table} {whereSql}";
                    cmd.CommandTimeout = 300;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader.GetValue(i);
                            }
                            results.Add(row);
                        }
                    }
                }
            }
            catch
            {
                // Preserve original empty catch block behavior
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }

            return results;
        }

        public List<Dictionary<string, object>> GetSearchResult_secondary(string query)
        {
            var results = new List<Dictionary<string, object>>();
            var conn = _dbpos.Database.GetDbConnection();

            try
            {
                conn.Open();
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = query;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader.GetValue(i);
                            }
                            results.Add(row);
                        }
                    }
                    return results;
                }
            }
            catch (Exception)
            {
                return results;
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }

        public List<Dictionary<string, object>> GetSearchResult_POS_Master(string query)
        {
            int timeoutSec = 300;
            var results = new List<Dictionary<string, object>>();
            var conn = _dbpos.Database.GetDbConnection();

            try
            {
                _dbpos.Database.SetCommandTimeout(timeoutSec);
                if (conn.State != ConnectionState.Open) conn.Open();

                using (var command = conn.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = timeoutSec;
                    command.CommandText = query;

                    using (var reader = command.ExecuteReader())
                    {
                        int colCount = reader.FieldCount;
                        var colNames = new string[colCount];
                        for (int i = 0; i < colCount; i++) colNames[i] = reader.GetName(i);

                        while (reader.Read())
                        {
                            var row = new Dictionary<string, object>(colCount, StringComparer.OrdinalIgnoreCase);
                            for (int i = 0; i < colCount; i++)
                                row[colNames[i]] = reader.IsDBNull(i) ? null : reader.GetValue(i);

                            results.Add(row);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return results;
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }

            return results;
        }

        public List<Dictionary<string, object>> GetSearchResult_DWDB(string query)
        {
            var results = new List<Dictionary<string, object>>();
            var conn = _dwdb.Database.GetDbConnection();

            try
            {
                conn.Open();
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = query;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader.GetValue(i);
                            }
                            results.Add(row);
                        }
                    }
                    return results;
                }
            }
            catch (Exception)
            {
                return results;
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }

        public List<Dictionary<string, object>> GetSearchResult_IFS(string query)
        {
            try
            {
                var results = new List<Dictionary<string, object>>();
                // Extracting the connection string from EF Core safely
                string connectionString = _dbIFS.Database.GetDbConnection().ConnectionString;

                using (var conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = query;
                        cmd.CommandType = CommandType.Text;

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var row = new Dictionary<string, object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                }
                                results.Add(row);
                            }
                            return results;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var results = new List<Dictionary<string, object>> { new Dictionary<string, object> { { "Error", ex.Message } } };
                return results;
            }
        }

        public List<Dictionary<string, object>> ExecProcWithJson(string procedure, string json, int timeoutSec = 300)
        {
            var results = new List<Dictionary<string, object>>();
            var connection = _db.Database.GetDbConnection();

            try
            {
                _db.Database.SetCommandTimeout(timeoutSec);
                if (connection.State != ConnectionState.Open) connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = procedure;
                    command.CommandTimeout = timeoutSec;

                    var p = command.CreateParameter();
                    p.ParameterName = "@Json";
                    p.DbType = DbType.String;
                    p.Value = (object)json ?? DBNull.Value;
                    command.Parameters.Add(p);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                                row[reader.GetName(i)] = reader.GetValue(i);

                            results.Add(row);
                        }
                    }
                }
            }
            finally
            {
                if (connection.State == ConnectionState.Open) connection.Close();
            }

            return results;
        }

        public List<List<Dictionary<string, object>>> ExecProcWithJsonMultiResult(string procedureName, string json, int timeoutSec)
        {
            var results = new List<List<Dictionary<string, object>>>();
            string connectionString = _db.Database.GetDbConnection().ConnectionString;

            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(procedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = timeoutSec;
                cmd.Parameters.Add(new SqlParameter("@Json", SqlDbType.NVarChar, -1) { Value = json });

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    do
                    {
                        var table = new List<Dictionary<string, object>>();
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                                row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            table.Add(row);
                        }
                        results.Add(table);
                    } while (reader.NextResult());
                }
            }

            return results;
        }

        public DataSet ExecuteStoredProcedure(string spName, Dictionary<string, object> parameters = null, int commandTimeout = 300)
        {
            DataSet ds = new DataSet();
            string connectionString = _db.Database.GetDbConnection().ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(spName, conn))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = commandTimeout;

                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                    }
                }

                adapter.Fill(ds);
            }

            return ds;
        }
    }
}