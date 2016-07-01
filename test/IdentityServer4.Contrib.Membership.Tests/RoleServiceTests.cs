// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer4.Contrib.Membership.Tests
{
    using System;
    using FluentAssertions;
    using Xunit;

    public class RoleServiceTests
    {
        [Fact]
        public void Ctor_RepoNull_ThrowsArgumentNullException()
        {
            Action ctor = () => new RoleService(null);

            ctor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("roleRepository");
        }

        //[Theory]
        //[InlineData(null)]
        //[InlineData("")]
        //public void GetRolesForUser_NullArgument_ThrowsArgumentNullException(string userName)
        //{
        //    // Arrange
        //    var repoFake = A.Fake<IRoleRepository>();
        //    var service = new RoleService(repoFake);

        //    // Act
        //    Action getRolesForUser = () => { var result = service.GetRolesForUser(userName).Result; };

        //    // Assert
        //    getRolesForUser.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("userName");
        //}

        //[Fact]
        //public void GetRolesForUser_RepoReturnsNullResult_ReturnsEmptyArray()
        //{
        //    // Arrange
        //    var repoFake = A.Fake<IRoleRepository>();

        //    var userName = "test@test.com";
        //    IEnumerable<string> repoResults = null;

        //    A.CallTo(() => repoFake.GetRoles(userName)).Returns(Task.FromResult(repoResults));

        //    var service = new RoleService(repoFake);

        //    // Act
        //    var result = service.GetRolesForUser(userName).Result;

        //    // Assert
        //    result.Should().BeEmpty();

        //    A.CallTo(() => repoFake.GetRoles(userName)).MustHaveHappened();
        //}

        //[Fact]
        //public void GetRolesForUser_RepoReturnsEmptyResult_ReturnsEmptyArray()
        //{
        //    // Arrange
        //    var repoFake = A.Fake<IRoleRepository>();

        //    var userName = "test@test.com";
        //    IEnumerable<string> repoResults = new string[0];

        //    A.CallTo(() => repoFake.GetRoles(userName)).Returns(Task.FromResult(repoResults));

        //    var service = new RoleService(repoFake);

        //    // Act
        //    var result = service.GetRolesForUser(userName).Result;

        //    // Assert
        //    result.Should().BeEmpty();

        //    A.CallTo(() => repoFake.GetRoles(userName)).MustHaveHappened();
        //}

        //[Fact]
        //public void GetRolesForUser_RepoReturnsResults_ReturnsResults()
        //{
        //    // Arrange
        //    var repoFake = A.Fake<IRoleRepository>();

        //    var userName = "test@test.com";
        //    IEnumerable<string> repoResults = new List<string> { "Role1", "Role2", "Role3" };

        //    A.CallTo(() => repoFake.GetRoles(userName)).Returns(Task.FromResult(repoResults));

        //    var service = new RoleService(repoFake);

        //    // Act
        //    var result = service.GetRolesForUser(userName).Result;

        //    // Assert
        //    result.Should().ContainInOrder("Role1", "Role2", "Role3");

        //    A.CallTo(() => repoFake.GetRoles(userName)).MustHaveHappened();
        //}
    }
}
