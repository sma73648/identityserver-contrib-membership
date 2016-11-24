// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Membership.DataAccess
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Membership;
    using Helpers;
    using Interfaces;

    /// <summary>Repository for the Roles Tables from a Membership Database</summary>
    public class RoleRepository : IRoleRepository
    {
        private readonly IMembershipContext membershipContext;
        private readonly MembershipOptions options;

        /// <summary>Constructor</summary>
        /// <param name="membershipContext">Membership Context</param>
        /// <param name="options">Membership Options</param>
        public RoleRepository(IMembershipContext membershipContext, MembershipOptions options)
        {
            this.membershipContext = membershipContext.ThrowIfNull(nameof(membershipContext));
            this.options = options.ThrowIfNull(nameof(options));
        }

        /// <summary>Get Application Roles for the Username</summary>
        /// <param name="username">Username</param>
        /// <returns>List of Application Roles</returns>
        public Task<IEnumerable<string>> GetRoles(string username)
        {
            return membershipContext.Execute(new QueryProc<string>("aspnet_UsersInRoles_GetRolesForUser")
                .Param("@ApplicationName", options.ApplicationName)
                .Param("@UserName", username)
                .Map(reader => reader.Get<string>("RoleName")));
        }
    }
}
