// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer4.Contrib.Membership
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Helpers;
    using Interfaces;

    /// <summary>Role Service that performs some of the RoleProvider read logic</summary>
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository roleRepository;

        /// <summary>Constructor</summary>
        /// <param name="roleRepository">Role Repository</param>
        public RoleService(IRoleRepository roleRepository)
        {
            this.roleRepository = roleRepository.ThrowIfNull(nameof(roleRepository));
        }

        /// <summary>Get Roles for the User</summary>
        /// <param name="userName">Username</param>
        /// <returns>Array of Roles - Empty Array if none found</returns>
        public async Task<string[]> GetRolesForUser(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException(nameof(userName));
            }

            var roles = await roleRepository.GetRoles(userName).ConfigureAwait(false);
            return roles?.ToArray() ?? new string[0];
        }
    }
}
