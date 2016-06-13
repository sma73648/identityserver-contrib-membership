// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Membership.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>Membership Database Context</summary>
    public interface IMembershipContext
    {
        /// <summary>Execute a Stored Procedure Query that returns results</summary>
        /// <typeparam name="T">Type of the Result</typeparam>
        /// <param name="args">Query Arguments</param>
        /// <returns>Set of Results</returns>
        Task<IEnumerable<T>> Execute<T>(IQueryProc<T> args);

        /// <summary>Execute a Stored Procedure Query that returns no results</summary>
        /// <param name="args">Query Arguments</param>
        Task Execute(IQueryProc args);
    }
}
