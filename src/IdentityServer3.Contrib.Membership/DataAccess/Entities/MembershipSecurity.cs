// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Membership.DataAccess.Entities
{
    using System;

    public class MembershipSecurity
    {
        public string Password { get; set; }

        public int PasswordFormat { get; set; }

        public string Salt { get; set; }

        public int FailedPasswordAttemptCount { get; set; }

        public int FailedPasswordAnswerAttemptCount { get; set; }

        public bool IsApproved { get; set; }

        public DateTime LastLoginDate { get; set; }

        public DateTime LastActivityDate { get; set; }
    }
}
