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

    public class MembershipRepositoryTests
    {
        [Fact]
        public void Ctor_ContextNull_ThrowsArgumentNullException()
        {
            Action ctor = () => new MembershipRepository(null, null);

            ctor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("context");
        }

        [Fact]
        public void Ctor_OptionsNull_ThrowsArgumentNullException()
        {
            var contextFake = A.Fake<IMembershipContext>();

            Action ctor = () => new MembershipRepository(contextFake, null);

            ctor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("options");
        }
    }
}
