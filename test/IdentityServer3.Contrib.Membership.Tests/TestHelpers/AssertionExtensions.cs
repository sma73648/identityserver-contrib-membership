// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Membership.Tests.TestHelpers
{
    using System.Security.Claims;
    using FluentAssertions;

    public static class AssertionExtensions
    {
        public static void ShouldBeEquivalentTo(this Claim claim, string claimType, string claimValue)
        {
            claim.ShouldBeEquivalentTo(new Claim(claimType, claimValue), opt => opt.Including(c => c.Type).Including(c => c.Value));
        }
    }
}
