// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Membership
{
    public class MembershipOptions
    {
        /// <summary>
        /// Default Constructor - Sets Default values
        /// </summary>
        public MembershipOptions()
        {
            MaxInvalidPasswordAttempts = 5;
            PasswordAttemptWindow = 10;
        }

        /// <summary>
        /// Membership Database Connection String
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Membership Database separates users into different applications - this determines which application data is being used
        /// for Authentication
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Determines whether the Role Provider tables should be used to determine the user roles
        /// </summary>
        public bool UseRoleProviderSource { get; set; }

        /// <summary>
        /// The maximum number of login attempts that can be attempted before the user is logged out
        /// </summary>
        public int MaxInvalidPasswordAttempts { get; set; }

        /// <summary>
        /// The number of seconds before a reattempt can be made
        /// </summary>
        public int PasswordAttemptWindow { get; set; }
    }
}
