// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Membership
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Core;
    using Core.Extensions;
    using Core.Models;
    using Core.Services.Default;
    using Helpers;
    using IdentityModel;
    using Interfaces;

    /// <summary>Identity Server User Service for Membership Database</summary>
    public class MembershipUserService : UserServiceBase
    {
        private readonly MembershipOptions options;
        private readonly IMembershipService membershipService;
        private readonly IRoleService roleService;

        /// <summary>Constructor</summary>
        /// <param name="options">Membership Options</param>
        /// <param name="membershipService">Membership Service</param>
        /// <param name="roleService">Role Service</param>
        public MembershipUserService(MembershipOptions options, IMembershipService membershipService, IRoleService roleService)
        {
            this.options = options.ThrowIfNull(nameof(options));
            this.membershipService = membershipService.ThrowIfNull(nameof(membershipService));
            this.roleService = roleService.ThrowIfNull(nameof(roleService));
        }

        /// <summary>Get Profile data</summary>
        /// <param name="context">Profile Data Request Context</param>
        public override async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject;
            subject.ThrowIfNull(nameof(subject));

            var id = subject.GetSubjectId();

            var key = ParseGuid(id);
            if (key == Guid.Empty) throw new ArgumentException("Invalid subject identifier");
            
            var user = await membershipService.GetUserAsync(key).ConfigureAwait(false);

            if (user == null) throw new ArgumentException("Invalid subject identifier");

            var requestedClaimTypes = context.RequestedClaimTypes.ToList();
            if (!context.AllClaimsRequested && requestedClaimTypes.Count == 0)
            {
                return;
            }

            var claims = GetClaimsFromAccount(user);
            if (!context.AllClaimsRequested && requestedClaimTypes.Count > 0)
            {
                claims = claims.Where(x => requestedClaimTypes.Contains(x.Type));
            }

            context.IssuedClaims = claims;
        }

        /// <summary>Gets the Claims from the Membership User</summary>
        /// <param name="user">Membership User</param>
        /// <returns>List of Claims</returns>
        protected virtual IEnumerable<Claim> GetClaimsFromAccount(MembershipUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, user.UserId.ToString("N")),
                new Claim(JwtClaimTypes.PreferredUserName, user.UserName),
                new Claim(JwtClaimTypes.Email, user.Email),

                new Claim(JwtClaimTypes.IdentityProvider, options.IdentityProvider),
                new Claim(JwtClaimTypes.AuthenticationTime, DateTime.UtcNow.ToEpochTime().ToString()),

                new Claim(MembershipClaimTypes.AccountCreated, user.AccountCreated.ToUtcEpoch().ToString()),
                new Claim(MembershipClaimTypes.LastActivity, user.LastActivity.ToUtcEpoch().ToString()),
                new Claim(MembershipClaimTypes.PasswordChanged, user.PasswordChanged.ToUtcEpoch().ToString())
            };

            // Get the roles ala the roles provider if required
            if (options.UseRoleProviderSource)
            {
                var roleClaims = roleService.GetRolesForUser(user.UserName).Result
                                            .Select(x => new Claim(Constants.ClaimTypes.Role, x));
                claims.AddRange(roleClaims);
            }

            return claims;
        }

        /// <summary>Authenticate the User locally</summary>
        /// <param name="context">Context</param>
        public override async Task AuthenticateLocalAsync(LocalAuthenticationContext context)
        {
            context.AuthenticateResult = null;

            if (string.IsNullOrWhiteSpace(context.UserName)) return;

            var user = await membershipService.GetUserAsync(context.UserName).ConfigureAwait(false);

            if (user == null || user.IsLockedOut)
            {
                return;
            }

            if (!await membershipService.ValidateUser(context.UserName, context.Password).ConfigureAwait(false))
            {
                return;
            }
            
            var claims = GetClaimsFromAccount(user);
            context.AuthenticateResult = new AuthenticateResult(user.UserId.ToString("N"), user.UserName, claims);
        }

        /// <summary>
        /// Determine if the context has a valid subject which needs to be a valid GUID
        /// </summary>
        /// <param name="context">Is Active Context</param>
        public override async Task IsActiveAsync(IsActiveContext context)
        {
            var id = context.Subject.GetSubjectId();

            var key = ParseGuid(id);
            if (key == Guid.Empty)
            {
                context.IsActive = false;
                return;
            }

            var user = await membershipService.GetUserAsync(key).ConfigureAwait(false);
            if (user == null)
            {
                context.IsActive = false;
                return;
            }

            context.IsActive = !user.IsLockedOut;                        
        }

        private Guid ParseGuid(string sub)
        {
            Guid key;
            if (!Guid.TryParse(sub, out key)) return Guid.Empty;
            return key;
        }
    }
}
