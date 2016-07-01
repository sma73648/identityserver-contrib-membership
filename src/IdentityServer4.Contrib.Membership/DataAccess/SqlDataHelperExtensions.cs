// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer4.Contrib.Membership.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    public static class SqlDataHelperExtensions
    {
        /// <summary>Get database values from a Dictionary</summary>
        /// <typeparam name="T">Type of value being retrieved</typeparam>
        /// <param name="dictionary">Dictionary containing database entries</param>
        /// <param name="name">Index value to be read</param>
        /// <returns>Returning value</returns>
        public static T Get<T>(this IDictionary<string, object> dictionary, string name)
        {
            var obj = dictionary[name];

            if (obj == null || obj == DBNull.Value)
            {
                return default(T);
            }

            return (T)obj;
        }

        /// <summary>Creates a Dictionary based on the current entry being read by a Data Reader</summary>
        /// <param name="reader">Data Reader</param>
        /// <returns>Dictionary containing Database entries</returns>
        public static IDictionary<string, object> ToDictionary(this IDataReader reader)
        {
            var results = new Dictionary<string, object>();

            for (var i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = i.ToString();
                }
                results[name] = reader.GetValue(i);
            }

            return results;
        }
    }
}
