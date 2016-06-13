// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Membership
{
    using System;
    using Core.Configuration;
    using Core.Services;
    using DataAccess;
    using Interfaces;

    public static class AppBuilderMembershipServiceExtensions
    {
        /// <summary>
        /// Register Identity Server to use a ASP.NET 2.0 Membership database to authenticate users
        /// </summary>
        /// <param name="factory">Identity Server Service Factory</param>
        /// <param name="options">Identity Server Membership Options</param>
        public static void UseMembershipService(this IdentityServerServiceFactory factory, MembershipOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.ConnectionString))
            {
                throw new ArgumentException(nameof(options.ConnectionString));
            }

            factory.Register(new Registration<MembershipOptions>(resolver => options));

            factory.Register(new Registration<IMembershipPasswordHasher, MembershipPasswordHasher>());

            factory.Register(new Registration<IMembershipContext, MembershipContext>());
            factory.Register(new Registration<IMembershipRepository, MembershipRepository>());
            factory.Register(new Registration<IRoleRepository, RoleRepository>());

            factory.Register(new Registration<IMembershipService, MembershipService>());
            factory.Register(new Registration<IRoleService, RoleService>());

            factory.UserService = new Registration<IUserService, MembershipUserService>();
        }
    }
}
