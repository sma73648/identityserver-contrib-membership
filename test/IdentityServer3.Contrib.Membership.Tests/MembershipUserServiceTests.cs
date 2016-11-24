// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Membership.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Membership;
    using Interfaces;
    using TestHelpers;
    using Core;
    using Core.Models;
    using FakeItEasy;
    using FluentAssertions;
    using IdentityModel;
    using Xunit;

    public class MembershipUserServiceTests
    {
        [Fact]
        public void Ctor_OptionsNull_ThrowsArgumentNullException()
        {
            Action ctor = () => new MembershipUserService(null, null, null);

            ctor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("options");
        }

        [Fact]
        public void Ctor_MembershipServiceNull_ThrowsArgumentNullException()
        {
            var options = new MembershipOptions();

            Action ctor = () => new MembershipUserService(options, null, null);

            ctor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("membershipService");
        }

        [Fact]
        public void Ctor_RoleServiceNull_ThrowsArgumentNullException()
        {
            var options = new MembershipOptions();
            var membershipService = A.Fake<IMembershipService>();

            Action ctor = () => new MembershipUserService(options, membershipService, null);

            ctor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("roleService");
        }

        [Fact]
        public void GetProfileDataAsync_SubjectNull_ThrowsArgumentNullException()
        {
            // Arrange
            var options = new MembershipOptions();
            var membershipService = A.Fake<IMembershipService>();
            var roleService = A.Fake<IRoleService>();

            var context = new ProfileDataRequestContext {Subject = null};

            var service = new MembershipUserService(options, membershipService, roleService);

            Action getProfileDataAsync = () => service.GetProfileDataAsync(context).Wait();

            // Act + Assert
            getProfileDataAsync.ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("subject");
        }

        [Fact]
        public void GetProfileDataAsync_SubjectClaimMissing_ThrowsInvalidOperationException()
        {
            // Arrange
            var options = new MembershipOptions();
            var membershipService = A.Fake<IMembershipService>();
            var roleService = A.Fake<IRoleService>();

            var context = new ProfileDataRequestContext
            {
                Subject = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()))
            };

            var service = new MembershipUserService(options, membershipService, roleService);

            Action getProfileDataAsync = () => service.GetProfileDataAsync(context).Wait();

            // Act + Assert
            getProfileDataAsync.ShouldThrow<InvalidOperationException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData("123")]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        [InlineData("00000000000000000000000000000000")]
        public void GetProfileDataAsync_SubjectClaimInvalid_ThrowsArgumentException(string subjectId)
        {
            // Arrange
            var options = new MembershipOptions();
            var membershipService = A.Fake<IMembershipService>();
            var roleService = A.Fake<IRoleService>();

            var context = new ProfileDataRequestContext
            {
                Subject = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(Constants.ClaimTypes.Subject, subjectId)
                }))
            };

            var service = new MembershipUserService(options, membershipService, roleService);

            Action getProfileDataAsync = () => service.GetProfileDataAsync(context).Wait();

            // Act + Assert
            getProfileDataAsync.ShouldThrow<ArgumentException>().WithMessage("Invalid subject identifier");
        }

        [Fact]
        public void GetProfileDataAsync_UserNotFound_ThrowsArgumentException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var options = new MembershipOptions();
            var membershipService = A.Fake<IMembershipService>();
            var roleService = A.Fake<IRoleService>();

            A.CallTo(() => membershipService.GetUserAsync(userId)).Returns(Task.FromResult((MembershipUser) null));

            var context = new ProfileDataRequestContext
            {
                Subject = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(Constants.ClaimTypes.Subject, userId.ToString("N"))
                }))
            };

            var service = new MembershipUserService(options, membershipService, roleService);

            Action getProfileDataAsync = () => service.GetProfileDataAsync(context).Wait();

            // Act
            getProfileDataAsync.ShouldThrow<ArgumentException>().WithMessage("Invalid subject identifier");

            // Assert
            A.CallTo(() => membershipService.GetUserAsync(userId)).MustHaveHappened();
        }

        [Fact]
        public void GetProfileDataAsync_UserFound_NoClaimsRequested_NoIssuedClaims()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var options = new MembershipOptions();
            var membershipService = A.Fake<IMembershipService>();
            var roleService = A.Fake<IRoleService>();

            var user = new MembershipUser();

            A.CallTo(() => membershipService.GetUserAsync(userId)).Returns(Task.FromResult(user));

            var context = new ProfileDataRequestContext
            {
                Subject = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(Constants.ClaimTypes.Subject, userId.ToString("N"))
                })),
                AllClaimsRequested = false,
                RequestedClaimTypes = new List<string>()
            };

            var service = new MembershipUserService(options, membershipService, roleService);

            // Act
            service.GetProfileDataAsync(context).Wait();

            // Assert
            context.IssuedClaims.Should().BeNullOrEmpty();

            A.CallTo(() => membershipService.GetUserAsync(userId));
        }

        [Fact]
        public void GetProfileDataAsync_UserFound_AllClaimsRequested_RolesNotUsed_NoIssuedClaims()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var options = new MembershipOptions();
            var membershipService = A.Fake<IMembershipService>();
            var roleService = A.Fake<IRoleService>();

            var user = new MembershipUser
            {
                UserId = userId,
                UserName = "username@test.com",
                Email = "email@test.com",
                IsLockedOut = false
            };

            A.CallTo(() => membershipService.GetUserAsync(userId)).Returns(Task.FromResult(user));

            var context = new ProfileDataRequestContext
            {
                Subject = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(Constants.ClaimTypes.Subject, userId.ToString("N"))
                })),
                AllClaimsRequested = true,
                RequestedClaimTypes = new List<string>()
            };

            var service = new MembershipUserService(options, membershipService, roleService);

            // Act
            service.GetProfileDataAsync(context).Wait();

            // Assert
            var issuedClaims = context.IssuedClaims.ToList();

            issuedClaims.Should().HaveCount(9);

            issuedClaims.ShouldContain(JwtClaimTypes.Subject, userId.ToString("N"));
            issuedClaims.ShouldContain(JwtClaimTypes.PreferredUserName, "username@test.com");
            issuedClaims.ShouldContain(JwtClaimTypes.Email, "email@test.com");

            A.CallTo(() => membershipService.GetUserAsync(userId));
        }

        [Fact]
        public void GetProfileDataAsync_UserFound_AllClaimsRequested_RolesIncluded_NoIssuedClaims()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var options = new MembershipOptions
            {
                UseRoleProviderSource = true,
                IdentityProvider = "idsvr"
            };
            var membershipService = A.Fake<IMembershipService>();
            var roleService = A.Fake<IRoleService>();

            var user = new MembershipUser
            {
                UserId = userId,
                UserName = "username@test.com",
                Email = "email@test.com",
                IsLockedOut = false
            };

            A.CallTo(() => membershipService.GetUserAsync(userId))
                .Returns(Task.FromResult(user));
            A.CallTo(() => roleService.GetRolesForUser("username@test.com"))
                .Returns(Task.FromResult(new[] {"role1", "role2", "role3"}));

            var context = new ProfileDataRequestContext
            {
                Subject = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(Constants.ClaimTypes.Subject, userId.ToString("N"))
                })),
                AllClaimsRequested = true,
                RequestedClaimTypes = new List<string>()
            };

            var service = new MembershipUserService(options, membershipService, roleService);

            // Act
            service.GetProfileDataAsync(context).Wait();

            // Assert
            var issuedClaims = context.IssuedClaims.ToList();

            issuedClaims.Should().HaveCount(12);

            issuedClaims.ShouldContain(JwtClaimTypes.Subject, userId.ToString("N"));
            issuedClaims.ShouldContain(JwtClaimTypes.PreferredUserName, "username@test.com");
            issuedClaims.ShouldContain(JwtClaimTypes.Email, "email@test.com");
            issuedClaims.ShouldContain(JwtClaimTypes.IdentityProvider, "idsvr");

            issuedClaims.ShouldContain(JwtClaimTypes.Role, "role1");
            issuedClaims.ShouldContain(JwtClaimTypes.Role, "role2");
            issuedClaims.ShouldContain(JwtClaimTypes.Role, "role3");

            A.CallTo(() => membershipService.GetUserAsync(userId));
        }

        [Fact]
        public void GetProfileDataAsync_UserFound_SomeClaimsRequested_RolesIncluded_NoIssuedClaims()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var options = new MembershipOptions { UseRoleProviderSource = true };
            var membershipService = A.Fake<IMembershipService>();
            var roleService = A.Fake<IRoleService>();

            var user = new MembershipUser
            {
                UserId = userId,
                UserName = "username@test.com",
                Email = "email@test.com",
                IsLockedOut = false
            };

            A.CallTo(() => membershipService.GetUserAsync(userId))
                .Returns(Task.FromResult(user));
            A.CallTo(() => roleService.GetRolesForUser("username@test.com"))
                .Returns(Task.FromResult(new[] { "role1", "role2", "role3" }));

            var context = new ProfileDataRequestContext
            {
                Subject = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(Constants.ClaimTypes.Subject, userId.ToString("N"))
                })),
                AllClaimsRequested = false,
                RequestedClaimTypes = new List<string>
                {
                    Constants.ClaimTypes.Email
                }
            };

            var service = new MembershipUserService(options, membershipService, roleService);

            // Act
            service.GetProfileDataAsync(context).Wait();

            // Assert
            var issuedClaims = context.IssuedClaims.ToList();

            issuedClaims.Should().HaveCount(1);
 
            issuedClaims[0].ShouldBeEquivalentTo(Constants.ClaimTypes.Email, "email@test.com");

            A.CallTo(() => membershipService.GetUserAsync(userId));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void AuthenticateLocalAsync_UsernameNullOrEmpty_AuthenticateResultIsNull(string username)
        {
            // Arrange
            var options = new MembershipOptions();
            var membershipService = A.Fake<IMembershipService>();
            var roleService = A.Fake<IRoleService>();

            var context = new LocalAuthenticationContext { UserName = username };            

            var service = new MembershipUserService(options, membershipService, roleService);

            // Act
            service.AuthenticateLocalAsync(context).Wait();

            // Assert
            context.AuthenticateResult.Should().BeNull();
        }

        [Fact]
        public void AuthenticateLocalAsync_UserNotFound_AuthenticateResultIsNull()
        {
            // Arrange
            var options = new MembershipOptions();
            var membershipService = A.Fake<IMembershipService>();
            var roleService = A.Fake<IRoleService>();

            var context = new LocalAuthenticationContext {UserName = "test@test.com"};

            A.CallTo(() => membershipService.GetUserAsync("test@test.com"))
             .Returns(Task.FromResult((MembershipUser) null));

            var service = new MembershipUserService(options, membershipService, roleService);

            // Act
            service.AuthenticateLocalAsync(context).Wait();

            // Assert
            context.AuthenticateResult.Should().BeNull();

            A.CallTo(() => membershipService.GetUserAsync("test@test.com")).MustHaveHappened();
        }

        [Fact]
        public void AuthenticateLocalAsync_UserLocked_AuthenticateResultIsNull()
        {
            // Arrange
            var options = new MembershipOptions();
            var membershipService = A.Fake<IMembershipService>();
            var roleService = A.Fake<IRoleService>();

            var context = new LocalAuthenticationContext { UserName = "test@test.com" };

            A.CallTo(() => membershipService.GetUserAsync("test@test.com"))
             .Returns(Task.FromResult(new MembershipUser { IsLockedOut = true }));

            var service = new MembershipUserService(options, membershipService, roleService);

            // Act
            service.AuthenticateLocalAsync(context).Wait();

            // Assert
            context.AuthenticateResult.Should().BeNull();

            A.CallTo(() => membershipService.GetUserAsync("test@test.com")).MustHaveHappened();
        }

        [Fact]
        public void AuthenticateLocalAsync_UserValidationFails_AuthenticateResultIsNull()
        {
            // Arrange
            var options = new MembershipOptions();
            var membershipService = A.Fake<IMembershipService>();
            var roleService = A.Fake<IRoleService>();

            var context = new LocalAuthenticationContext { UserName = "test@test.com", Password = "password123" };

            A.CallTo(() => membershipService.GetUserAsync("test@test.com"))
             .Returns(Task.FromResult(new MembershipUser { IsLockedOut = false }));

            A.CallTo(() => membershipService.ValidateUser("test@test.com", "password123"))
             .Returns(Task.FromResult(false));

            var service = new MembershipUserService(options, membershipService, roleService);

            // Act
            service.AuthenticateLocalAsync(context).Wait();

            // Assert
            context.AuthenticateResult.Should().BeNull();

            A.CallTo(() => membershipService.GetUserAsync("test@test.com")).MustHaveHappened();
            A.CallTo(() => membershipService.ValidateUser("test@test.com", "password123")).MustHaveHappened();
        }

        [Fact]
        public void AuthenticateLocalAsync_UserValid_RolesIncluded_AuthenticateResultSet()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var options = new MembershipOptions { UseRoleProviderSource = true };
            var membershipService = A.Fake<IMembershipService>();
            var roleService = A.Fake<IRoleService>();

            var context = new LocalAuthenticationContext { UserName = "test@test.com", Password = "password123" };

            A.CallTo(() => membershipService.GetUserAsync("test@test.com"))
             .Returns(Task.FromResult(new MembershipUser
             {
                 UserId = userId,
                 UserName = "test@test.com",
                 Email = "email@test.com",
                 IsLockedOut = false,
                 AccountCreated = new DateTime(2010, 10, 01),

             }));

            A.CallTo(() => membershipService.ValidateUser("test@test.com", "password123"))
             .Returns(Task.FromResult(true));

            A.CallTo(() => roleService.GetRolesForUser("test@test.com"))
             .Returns(Task.FromResult(new[] {"Role1", "Role2", "Role3"}));

            var service = new MembershipUserService(options, membershipService, roleService);

            // Act
            service.AuthenticateLocalAsync(context).Wait();

            // Assert
            context.AuthenticateResult.Should().NotBeNull();
            context.AuthenticateResult.HasSubject.Should().BeTrue();

            var issuedClaims = context.AuthenticateResult.User.Claims.ToList();
            issuedClaims.Should().HaveCount(14);

            issuedClaims.ShouldContain(JwtClaimTypes.Subject, userId.ToString("N"));
            issuedClaims.ShouldContain(ClaimTypes.NameIdentifier, userId.ToString("N"));
            issuedClaims.ShouldContain(JwtClaimTypes.Name, "test@test.com");
            issuedClaims.ShouldContain(JwtClaimTypes.AuthenticationMethod, "password");
            issuedClaims.ShouldContain(JwtClaimTypes.IdentityProvider, "idsrv");
            issuedClaims.ShouldContain(JwtClaimTypes.AuthenticationTime);
            issuedClaims.ShouldContain(JwtClaimTypes.PreferredUserName, "test@test.com");
            issuedClaims.ShouldContain(JwtClaimTypes.Email, "email@test.com");
            issuedClaims.ShouldContain(MembershipClaimTypes.AccountCreated);
            issuedClaims.ShouldContain(MembershipClaimTypes.LastActivity);
            issuedClaims.ShouldContain(MembershipClaimTypes.PasswordChanged);
            issuedClaims.ShouldContain(Constants.ClaimTypes.Role, "Role1");
            issuedClaims.ShouldContain(Constants.ClaimTypes.Role, "Role2");
            issuedClaims.ShouldContain(Constants.ClaimTypes.Role, "Role3");

            A.CallTo(() => membershipService.GetUserAsync("test@test.com")).MustHaveHappened();
            A.CallTo(() => membershipService.ValidateUser("test@test.com", "password123")).MustHaveHappened();
            A.CallTo(() => roleService.GetRolesForUser("test@test.com")).MustHaveHappened();
        }

        [Fact]
        public void AuthenticateLocalAsync_UserValid_RolesNotIncluded_AuthenticateResultSet()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var options = new MembershipOptions();
            var membershipService = A.Fake<IMembershipService>();
            var roleService = A.Fake<IRoleService>();

            var context = new LocalAuthenticationContext { UserName = "test@test.com", Password = "password123" };

            A.CallTo(() => membershipService.GetUserAsync("test@test.com"))
             .Returns(Task.FromResult(new MembershipUser
             {
                 UserId = userId,
                 UserName = "test@test.com",
                 Email = "email@test.com",
                 IsLockedOut = false
             }));

            A.CallTo(() => membershipService.ValidateUser("test@test.com", "password123"))
             .Returns(Task.FromResult(true));

            var service = new MembershipUserService(options, membershipService, roleService);

            // Act
            service.AuthenticateLocalAsync(context).Wait();

            // Assert
            context.AuthenticateResult.Should().NotBeNull();
            context.AuthenticateResult.HasSubject.Should().BeTrue();

            var issuedClaims = context.AuthenticateResult.User.Claims.ToList();
            issuedClaims.Should().HaveCount(11);

            issuedClaims.ShouldContain(Constants.ClaimTypes.Subject, userId.ToString("N"));
            issuedClaims.ShouldContain(Constants.ClaimTypes.Name, "test@test.com");
            issuedClaims.ShouldContain(Constants.ClaimTypes.AuthenticationMethod, "password");
            issuedClaims.ShouldContain(Constants.ClaimTypes.IdentityProvider, "idsrv");
            issuedClaims.ShouldContain(Constants.ClaimTypes.AuthenticationTime);
            issuedClaims.ShouldContain(Constants.ClaimTypes.PreferredUserName, "test@test.com");
            issuedClaims.ShouldContain(Constants.ClaimTypes.Email, "email@test.com");

            A.CallTo(() => membershipService.GetUserAsync("test@test.com")).MustHaveHappened();
            A.CallTo(() => membershipService.ValidateUser("test@test.com", "password123")).MustHaveHappened();
        }

        [Fact]
        public void IsActiveAsync_SubjectNull_ContextIsNotActive()
        {
            // Arrange
            var options = new MembershipOptions();
            var membershipService = A.Fake<IMembershipService>();
            var roleService = A.Fake<IRoleService>();

            var context = new IsActiveContext(new ClaimsPrincipal(new ClaimsIdentity()), new Client());

            var service = new MembershipUserService(options, membershipService, roleService);

            Action isActiveAsync = () => service.IsActiveAsync(context).Wait();

            // Act + Assert
            isActiveAsync.ShouldThrow<InvalidOperationException>().And.Message.Should().Be("sub claim is missing");
        }

        [Theory]
        [InlineData("")]
        [InlineData("123")]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        [InlineData("00000000000000000000000000000000")]
        public void IsActiveAsync_SubjectClaimInvalid_ContextIsNotActive(string subjectId)
        {
            // Arrange
            var options = new MembershipOptions();
            var membershipService = A.Fake<IMembershipService>();
            var roleService = A.Fake<IRoleService>();

            var context = new IsActiveContext(
                new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new Claim(Constants.ClaimTypes.Subject, subjectId) })), 
                new Client()
            );

            var service = new MembershipUserService(options, membershipService, roleService);

            // Act
            service.IsActiveAsync(context).Wait();

            // Assert
            context.IsActive.Should().BeFalse();
        }

        [Fact]
        public void IsActiveAsync_UserNotFound_ContextIsNotActive()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var options = new MembershipOptions();
            var membershipService = A.Fake<IMembershipService>();
            var roleService = A.Fake<IRoleService>();

            A.CallTo(() => membershipService.GetUserAsync(userId)).Returns(Task.FromResult((MembershipUser)null));

            var context = new IsActiveContext(
                new ClaimsPrincipal(
                    new ClaimsIdentity(new List<Claim>
                    {
                        new Claim(Constants.ClaimTypes.Subject, userId.ToString("N"))
                    })),
                new Client()
            );

            var service = new MembershipUserService(options, membershipService, roleService);

            // Act
            service.IsActiveAsync(context).Wait();

            // Assert
            context.IsActive.Should().BeFalse();

            A.CallTo(() => membershipService.GetUserAsync(userId)).MustHaveHappened();
        }

        [Fact]
        public void IsActiveAsync_UserFoundButInactive_ContextIsNotActive()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var options = new MembershipOptions();
            var membershipService = A.Fake<IMembershipService>();
            var roleService = A.Fake<IRoleService>();

            var user = new MembershipUser { IsLockedOut = true };

            A.CallTo(() => membershipService.GetUserAsync(userId)).Returns(Task.FromResult(user));

            var context = new IsActiveContext(
                new ClaimsPrincipal(
                    new ClaimsIdentity(new List<Claim>
                    {
                        new Claim(Constants.ClaimTypes.Subject, userId.ToString("N"))
                    })),
                new Client()
            );

            var service = new MembershipUserService(options, membershipService, roleService);

            // Act
            service.IsActiveAsync(context).Wait();

            // Assert
            context.IsActive.Should().BeFalse();

            A.CallTo(() => membershipService.GetUserAsync(userId)).MustHaveHappened();
        }

        [Fact]
        public void IsActiveAsync_UserFoundAndActive_ContextIsActive()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var options = new MembershipOptions();
            var membershipService = A.Fake<IMembershipService>();
            var roleService = A.Fake<IRoleService>();

            var user = new MembershipUser { IsLockedOut = false };

            A.CallTo(() => membershipService.GetUserAsync(userId)).Returns(Task.FromResult(user));

            var context = new IsActiveContext(
                new ClaimsPrincipal(
                    new ClaimsIdentity(new List<Claim>
                    {
                        new Claim(Constants.ClaimTypes.Subject, userId.ToString("N"))
                    })),
                new Client()
            );

            var service = new MembershipUserService(options, membershipService, roleService);

            // Act
            service.IsActiveAsync(context).Wait();

            // Assert
            context.IsActive.Should().BeTrue();

            A.CallTo(() => membershipService.GetUserAsync(userId)).MustHaveHappened();
        }
    }
}
