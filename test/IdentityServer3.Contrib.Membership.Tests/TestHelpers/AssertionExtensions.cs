// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Membership.Tests.TestHelpers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using FluentAssertions;

    public static class AssertionExtensions
    {
        public static void ShouldContain(this IEnumerable<Claim> claims, string claimType, string claimValue)
        {
            var claim = claims.FirstOrDefault(x => x.Type == claimType && x.Value == claimValue);
            claim.Should().NotBeNull();
            claim.ShouldBeEquivalentTo(claimType, claimValue);
        }

        public static void ShouldContain(this IEnumerable<Claim> claims, string claimType)
        {
            var claim = claims.FirstOrDefault(x => x.Type == claimType);

            claim.Should().NotBeNull();
            claim.ShouldBeClaimType(claimType);
        }

        public static void ShouldBeEquivalentTo(this Claim claim, string claimType, string claimValue)
        {
            claim.ShouldBeEquivalentTo(new Claim(claimType, claimValue), opt => opt.Including(c => c.Type).Including(c => c.Value));
        }

        public static void ShouldBeClaimType(this Claim claim, string claimType)
        {
            claim.Type.Should().Be(claimType);
        }
    }
}
