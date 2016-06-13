// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Membership.DataAccess
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using Membership;
    using Helpers;
    using Interfaces;

    /// <summary>Membership Database Context</summary>
    public class MembershipContext : IMembershipContext
    {
        private readonly MembershipOptions options;

        /// <summary>Constructor</summary>
        /// <param name="options">Membership Options</param>
        public MembershipContext(MembershipOptions options)
        {
            this.options = options.ThrowIfNull(nameof(options));
            options.ConnectionString.ThrowIfNull(nameof(options.ConnectionString));
        }

        /// <summary>Execute a Stored Procedure Query that returns results</summary>
        /// <typeparam name="T">Type of the Result</typeparam>
        /// <param name="args">Query Arguments</param>
        /// <returns>Set of Results</returns>
        public async Task<IEnumerable<T>> Execute<T>(IQueryProc<T> args)
        {
            args.ResultMapper.ThrowIfNull(nameof(args.ResultMapper));
            
            using (var connection = new SqlConnection(options.ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                using (var cmd = CreateCommand(connection, args.ProcName, args.Parameters))
                {
                    var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

                    var list = new List<T>();
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        list.Add(args.ResultMapper(reader.ToDictionary()));
                    }
                    return list;
                }
            }
        }

        /// <summary>Execute a Stored Procedure Query that returns no results</summary>
        /// <param name="args">Query Arguments</param>
        public async Task Execute(IQueryProc args)
        {
            using (var connection = new SqlConnection(options.ConnectionString))                
            {
                await connection.OpenAsync().ConfigureAwait(false);

                using (var cmd = CreateCommand(connection, args.ProcName, args.Parameters))
                {
                    await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Uses a stored proc!")]
        private SqlCommand CreateCommand(SqlConnection connection, string procName, IDictionary<string, object> parameters)
        {
            var cmd = new SqlCommand(procName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    var sqlParameter = cmd.CreateParameter();

                    sqlParameter.ParameterName = parameter.Key;
                    sqlParameter.Value = parameter.Value;

                    cmd.Parameters.Add(sqlParameter);
                }
            }
            return cmd;
        }
    }
}
