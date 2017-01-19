// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Membership.Interfaces
{
    using System;
    using System.Collections.Generic;

    public interface IQueryProc
    {
        string ProcName { get; }

        IDictionary<string, object> Parameters { get; }

        IQueryProc Param(string paramName, object value);
    }

    public interface IQueryProc<T> : IQueryProc
    {           
        new IQueryProc<T> Param(string paramName, object value);

        Func<IList<KeyValuePair<string, object>>, T> ResultMapper { get; }

        IQueryProc<T> Map(Func<IList<KeyValuePair<string, object>>, T> mapper);
    }
}
