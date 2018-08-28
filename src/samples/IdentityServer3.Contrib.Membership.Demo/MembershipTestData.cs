// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Membership.Demo
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;

    class MembershipTestData
    {
        public static void SetUp(string dbName, string applicationName)
        {
            if (DoesUserExist(dbName, applicationName)) return;

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings[dbName].ConnectionString))
            {
                connection.Open();

                var cmd = new SqlCommand("aspnet_Membership_CreateUser", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                var sqlParameter = cmd.CreateParameter();
                sqlParameter.ParameterName = "@ApplicationName";
                sqlParameter.Value = applicationName;
                cmd.Parameters.Add(sqlParameter);

                sqlParameter = cmd.CreateParameter();
                sqlParameter.ParameterName = "@UserName";
                sqlParameter.Value = "test@test.com";
                cmd.Parameters.Add(sqlParameter);

                sqlParameter = cmd.CreateParameter();
                sqlParameter.ParameterName = "@Password";
                sqlParameter.Value = "password123";
                cmd.Parameters.Add(sqlParameter);

                sqlParameter = cmd.CreateParameter();
                sqlParameter.ParameterName = "@PasswordSalt";
                sqlParameter.Value = "";
                cmd.Parameters.Add(sqlParameter);

                sqlParameter = cmd.CreateParameter();
                sqlParameter.ParameterName = "@Email";
                sqlParameter.Value = "test@test.com";
                cmd.Parameters.Add(sqlParameter);

                sqlParameter = cmd.CreateParameter();
                sqlParameter.ParameterName = "@PasswordQuestion";
                sqlParameter.Value = "Test Question";
                cmd.Parameters.Add(sqlParameter);

                sqlParameter = cmd.CreateParameter();
                sqlParameter.ParameterName = "@PasswordAnswer";
                sqlParameter.Value = "Test Answer";
                cmd.Parameters.Add(sqlParameter);

                sqlParameter = cmd.CreateParameter();
                sqlParameter.ParameterName = "@IsApproved";
                sqlParameter.Value = true;
                cmd.Parameters.Add(sqlParameter);

                sqlParameter = cmd.CreateParameter();
                sqlParameter.ParameterName = "@CurrentTimeUtc";
                sqlParameter.DbType = DbType.DateTime;
                sqlParameter.Value = DateTime.UtcNow;
                cmd.Parameters.Add(sqlParameter);

                sqlParameter = cmd.CreateParameter();
                sqlParameter.ParameterName = "@UserId";
                sqlParameter.DbType = DbType.Guid;
                sqlParameter.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(sqlParameter);

                cmd.ExecuteNonQuery();
            }
        }

        private static bool DoesUserExist(string dbName, string applicationName)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings[dbName].ConnectionString))
            {
                connection.Open();

                // Check that the test user exists
                var cmd = new SqlCommand("aspnet_Membership_FindUsersByEmail", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                var sqlParameter = cmd.CreateParameter();

                sqlParameter.ParameterName = "@ApplicationName";
                sqlParameter.Value = applicationName;
                cmd.Parameters.Add(sqlParameter);

                sqlParameter = cmd.CreateParameter();
                sqlParameter.ParameterName = "@EmailToMatch";
                sqlParameter.Value = "test@test.com";
                cmd.Parameters.Add(sqlParameter);

                sqlParameter = cmd.CreateParameter();
                sqlParameter.ParameterName = "@PageIndex";
                sqlParameter.Value = 1;
                cmd.Parameters.Add(sqlParameter);

                sqlParameter = cmd.CreateParameter();
                sqlParameter.ParameterName = "@PageSize";
                sqlParameter.Value = 1;
                cmd.Parameters.Add(sqlParameter);

                return cmd.ExecuteScalar() as int? >= 1;
            }
        }
    }
}
