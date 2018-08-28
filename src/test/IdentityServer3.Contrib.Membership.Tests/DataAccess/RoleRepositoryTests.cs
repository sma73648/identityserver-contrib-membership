// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Membership.Tests.DataAccess
{
    using System;
    using Membership.DataAccess;
    using Interfaces;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class RoleRepositoryTests
    {
        [Fact]
        public void Ctor_ContextNull_ThrowsArgumentNullException()
        {
            Action ctor = () => new RoleRepository(null, null);

            ctor.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("membershipContext");
        }

        [Fact]
        public void Ctor_OptionsNull_ThrowsArgumentNullException()
        {
            var contextFake = A.Fake<IMembershipContext>();

            Action ctor = () => new RoleRepository(contextFake, null);

            ctor.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("options");
        }
    }
}
