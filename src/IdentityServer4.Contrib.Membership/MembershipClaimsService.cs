// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer4.Contrib.Membership
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Helpers;
    using IdentityModel;
    using Interfaces;

    public class MembershipClaimsService : IMembershipClaimsService
    {
        private readonly MembershipOptions options;
        private readonly IMembershipService membershipService;
        private readonly IRoleService roleService;

        public MembershipClaimsService(IMembershipService membershipService, IRoleService roleService, MembershipOptions options)
        {
            this.membershipService = membershipService.ThrowIfNull(nameof(membershipService));
            this.roleService = roleService.ThrowIfNull(nameof(roleService));
            this.options = options.ThrowIfNull(nameof(options));
        }

        public async Task<IEnumerable<Claim>> GetClaimsFromAccount(string username)
        {
            var user = await membershipService.GetUserAsync(username).ConfigureAwait(false);
            return user == null ? Enumerable.Empty<Claim>() : await GetClaimsFromAccount(user).ConfigureAwait(false);
        }

        /// <summary>Gets the Claims from the Membership User</summary>
        /// <param name="user">Membership User</param>
        /// <returns>List of Claims</returns>
        public async Task<IEnumerable<Claim>> GetClaimsFromAccount(MembershipUser user)
        {
            var claims = new List<Claim>{
                new Claim(JwtClaimTypes.Subject, user.GetSubjectId()),
                new Claim(JwtClaimTypes.Name, user.UserName),
                new Claim(JwtClaimTypes.Email, user.Email),

                new Claim(JwtClaimTypes.IdentityProvider, options.IdentityProvider),
                new Claim(JwtClaimTypes.AuthenticationTime, DateTime.UtcNow.ToEpochTime().ToString()),

                new Claim(MembershipClaimTypes.AccountCreated, user.AccountCreated.ToEpochTime().ToString()),
                new Claim(MembershipClaimTypes.LastActivity, user.LastActivity.ToEpochTime().ToString()),
                new Claim(MembershipClaimTypes.PasswordChanged, user.PasswordChanged.ToEpochTime().ToString())
            };

            // Get the roles ala the roles provider if required
            if (options.UseRoleProviderSource)
            {
                var roleClaims = await roleService.GetRolesForUser(user.UserName).ConfigureAwait(false);
                claims.AddRange(roleClaims.Select(x => new Claim(JwtClaimTypes.Role, x)));
            }

            return claims;
        }
    }
}
