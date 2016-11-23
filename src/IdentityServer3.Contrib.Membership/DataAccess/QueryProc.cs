// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Membership.DataAccess
{
    using System;
    using System.Collections.Generic;
    using Interfaces;

    /// <summary>Stored Procedure Query Arguments for Non-Query</summary>
    public class QueryProc : IQueryProc
    {
        /// <summary>Constructor</summary>
        /// <param name="procName">Stored Procedure Name</param>
        public QueryProc(string procName)
        {
            ProcName = procName;
        }

        /// <summary>Stored Procedure Name</summary>
        public string ProcName { get; }

        /// <summary>Query Parameters</summary>
        public IDictionary<string, object> Parameters { get; } = new Dictionary<string, object>();

        /// <summary>Appends a Parameter</summary>
        /// <param name="paramName">Parameter Name</param>
        /// <param name="value">Parameter Value</param>
        /// <returns>Query Proc</returns>
        public IQueryProc Param(string paramName, object value)
        {
            Parameters[paramName] = value;
            return this;
        }
    }

    /// <summary>Stored Procedure Query Arguments</summary>
    /// <typeparam name="T">Type of result</typeparam>
    public class QueryProc<T> : QueryProc, IQueryProc<T>
    {
        /// <summary>Constructor</summary>
        /// <param name="procName">Stored Procedure Name</param>
        public QueryProc(string procName)
            : base(procName)
        {
        } 

        /// <summary>Result Mapper</summary>
        public Func<IList<KeyValuePair<string, object>>, T> ResultMapper { get; private set; }

        /// <summary>Sets the Result Mapper</summary>
        /// <param name="mapper">Result Mapper</param>
        /// <returns>Query Proc</returns>
        public IQueryProc<T> Map(Func<IList<KeyValuePair<string, object>>, T> mapper)
        {
            ResultMapper = mapper;
            return this;
        }

        /// <summary>Appends a Parameter</summary>
        /// <param name="paramName">Parameter Name</param>
        /// <param name="value">Parameter Value</param>
        /// <returns>Query Proc</returns>
        public new IQueryProc<T> Param(string paramName, object value)
        {
            Parameters[paramName] = value;
            return this;
        }
    }
}
