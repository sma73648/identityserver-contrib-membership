// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer4.Contrib.Membership
{
    using System;
    using DataAccess;
    using Interfaces;
    using Microsoft.Extensions.DependencyInjection;
    using Services;

    public static class AppBuilderMembershipServiceExtensions
    {
        /// <summary>
        /// Register Identity Server to use a ASP.NET 2.0 Membership database to authenticate users
        /// </summary>
        /// <param name="builder">Identity Server Service builder</param>
        /// <param name="options">Identity Server Membership Options</param>
        public static IIdentityServerBuilder AddMembershipService(this IIdentityServerBuilder builder, MembershipOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.ConnectionString))
            {
                throw new ArgumentException(nameof(options.ConnectionString));
            }

            builder.Services.AddSingleton(options);

            builder.Services.AddTransient<IMembershipPasswordHasher, MembershipPasswordHasher>();

            builder.Services.AddTransient<IMembershipContext, MembershipContext>();
            builder.Services.AddTransient<IMembershipRepository, MembershipRepository>();
            builder.Services.AddTransient<IRoleRepository, RoleRepository>();

            builder.Services.AddTransient<IMembershipService, MembershipService>();
            builder.Services.AddTransient<IRoleService, RoleService>();
            builder.Services.AddTransient<IMembershipClaimsService, MembershipClaimsService>();

            builder.Services.AddTransient<IProfileService, MembershipProfileService>();

            return builder;
        }
    }
}
