// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer4.Contrib.Membership
{
    using System;

    /// <summary>Membership User presented to Identity Server</summary>
    public class MembershipUser
    {
        /// <summary>User Unique Identifier</summary>
        public Guid UserId { get; set; }

        /// <summary>Username</summary>
        public string UserName { get; set; }

        /// <summary>E-mail Address of the User</summary>
        public string Email { get; set; }

        /// <summary>Is the user locked out</summary>
        public bool IsLockedOut { get; set; }

        /// <summary>Account Created</summary>
        public DateTime AccountCreated { get; set; }

        /// <summary>Last Activity</summary>
        public DateTime LastActivity { get; set; }

        /// <summary>Password Changed</summary>
        public DateTime PasswordChanged { get; set; }
    }
}
