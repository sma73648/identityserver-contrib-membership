// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Membership.Tests
{
    using System;
    using System.Threading.Tasks;
    using Membership;
    using Interfaces;
    using FakeItEasy;
    using FluentAssertions;
    using Membership.DataAccess.Entities;
    using Xunit;

    public class MembershipServiceTests
    {
        [Fact]
        public void Ctor_RepoNull_ThrowsArgumentNullException()
        {
            Action ctor = () => new MembershipService(null, null);

            ctor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("membershipRepository");
        }

        [Fact]
        public void Ctor_HasherNull_TrhowsArgumentNullException()
        {
            var repoFake = A.Fake<IMembershipRepository>();

            Action ctor = () => new MembershipService(repoFake, null);

            ctor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("membershipPasswordHasher");
        }

        [Fact]
        public void GetUserAsync_UserNotFound_ReturnsNull()
        {
            // Arrange
            var repoFake = A.Fake<IMembershipRepository>();
            var hasherFake = A.Fake<IMembershipPasswordHasher>();

            var userId = Guid.NewGuid();

            Membership repoResult = null;
            A.CallTo(() => repoFake.FindUserById(userId)).Returns(Task.FromResult(repoResult));

            var service = new MembershipService(repoFake, hasherFake);

            // Act
            var result = service.GetUserAsync(userId).Result;
            
            // Assert
            result.Should().BeNull();

            A.CallTo(() => repoFake.FindUserById(userId)).MustHaveHappened();
        }

        [Fact]
        public void GetUserAsync_UserFound_ReturnsMembershipUser()
        {
            // Arrange
            var repoFake = A.Fake<IMembershipRepository>();
            var hasherFake = A.Fake<IMembershipPasswordHasher>();

            var userId = Guid.NewGuid();

            A.CallTo(() => repoFake.FindUserById(userId)).Returns(Task.FromResult(new Membership
            {
                UserId = userId,
                UserName = "testuser123",
                Email = "test@test.com",
                IsLockedOut = true
            }));

            var service = new MembershipService(repoFake, hasherFake);

            // Act
            var result = service.GetUserAsync(userId).Result;
            
            // Assert
            result.UserId.Should().Be(userId);
            result.UserName.Should().Be("testuser123");
            result.Email.Should().Be("test@test.com");
            result.IsLockedOut.Should().Be(true);

            A.CallTo(() => repoFake.FindUserById(userId)).MustHaveHappened();
        }
    }
}
