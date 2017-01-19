// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer4.Contrib.Membership.DataAccess.Entities
{
    using System;

    public class Membership
    {
        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string PasswordQuestion { get; set; }

        public bool IsApproved { get; set; }

        public bool IsLockedOut { get; set; }

        public string Comment { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime LastLoginDate { get; set; }

        public DateTime LastActivityDate { get; set; }

        public DateTime LastPasswordChangedDate { get; set; }

        public DateTime LastLockoutDate { get; set; }
    }
}
