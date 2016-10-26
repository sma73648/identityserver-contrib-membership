// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer4.Contrib.Membership.Helpers
{
    using System.Security.Claims;

    public static class MembershipUserExtensions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimsPrincipal"/> class with Claims from the Membership User
        /// </summary>
        /// <param name="user">Membership User</param>
        /// <param name="identityProvider">Identity Provider Name</param>
        /// <param name="claims">List of additional claims</param>
        /// <returns>Claims Principal</returns>
        public static ClaimsPrincipal Create(this MembershipUser user, string identityProvider, params Claim[] claims)
        {
            return IdentityServerPrincipal.Create(user.GetSubjectId(), user.UserName, identityProvider, claims);
        }

        /// <summary>
        /// Gets the Subject Id for the Membership User
        /// </summary>
        /// <param name="user">Membership User</param>
        /// <returns>Subject Id</returns>
        public static string GetSubjectId(this MembershipUser user)
        {
            return user.UserId.ToString("N");
        }
    }
}
