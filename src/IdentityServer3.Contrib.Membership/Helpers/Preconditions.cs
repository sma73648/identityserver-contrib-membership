// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Membership.Helpers
{
    using System;

    public static class Preconditions
    {
        public static T ThrowIfNull<T>(this T param, string nameOfParam)
        {
            if (param == null) throw new ArgumentNullException(nameOfParam);
            return param;
        }
    }
}
