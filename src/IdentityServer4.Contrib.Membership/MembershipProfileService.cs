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
    using Models;
    using Services;
    using Extensions;

    public class MembershipProfileService : IProfileService
    {
        private readonly IMembershipService membershipService;
        private readonly IClaimsService claimsService;

        /// <summary>Constructor</summary>
        /// <param name="membershipService">Membership Service</param>
        /// <param name="claimsService">Claims Service</param>
        public MembershipProfileService(IMembershipService membershipService, IClaimsService claimsService)
        {
            this.membershipService = membershipService.ThrowIfNull(nameof(membershipService));
            this.claimsService = claimsService.ThrowIfNull(nameof(claimsService));
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
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

            var claims = claimsService.GetClaimsFromAccount(user);
            if (!context.AllClaimsRequested && requestedClaimTypes.Count > 0)
            {
                claims = claims.Where(x => requestedClaimTypes.Contains(x.Type));
            }

            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
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

        private static Guid ParseGuid(string sub)
        {
            Guid key;
            if (!Guid.TryParse(sub, out key)) return Guid.Empty;
            return key;
        }
    }
}
