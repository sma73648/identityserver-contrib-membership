// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Membership.DataAccess
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Membership;
    using Helpers;
    using Interfaces;
    using Entities;

    /// <summary>Repository for the Membership and User Tables from a Membership Database</summary>
    public class MembershipRepository : IMembershipRepository
    {
        private readonly IMembershipContext context;
        private readonly MembershipOptions options;

        /// <summary>Constructor</summary>
        /// <param name="context">Membership Context</param>
        /// <param name="options">Membership Options</param>
        public MembershipRepository(IMembershipContext context, MembershipOptions options)
        {
            this.context = context.ThrowIfNull(nameof(context));
            this.options = options.ThrowIfNull(nameof(options));
        }
        
        /// <summary>Find user details by Id</summary>
        /// <param name="id">Unqique Identifier</param>
        /// <returns>Membership entry</returns>
        public async Task<Membership> FindUserById(Guid id)
        {
            return (await context.Execute(new QueryProc<Membership>("aspnet_Membership_GetUserByUserId")
                    .Param("@UserId", id)
                    .Param("@CurrentTimeUtc", DateTime.UtcNow)
                    .Param("@UpdateLastActivity", true)
                    .Map(reader => new Membership
                    {
                        UserId = id,
                        UserName = reader.Get<string>("UserName"),
                        Email = reader.Get<string>("Email"),
                        PasswordQuestion = reader.Get<string>("PasswordQuestion"),
                        IsApproved = reader.Get<bool>("IsApproved"),
                        IsLockedOut = reader.Get<bool>("IsLockedOut"),
                        Comment = reader.Get<string>("Comment"),
                        CreateDate = reader.Get<DateTime>("CreateDate"),
                        LastLoginDate = reader.Get<DateTime>("LastLoginDate"),
                        LastActivityDate = reader.Get<DateTime>("LastActivityDate"),
                        LastPasswordChangedDate = reader.Get<DateTime>("LastPasswordChangedDate"),
                        LastLockoutDate = reader.Get<DateTime>("LastLockoutDate")
                    })
            ).ConfigureAwait(false)).SingleOrDefault();
        }

        /// <summary>Find user details by Username</summary>
        /// <param name="username">Username</param>
        /// <returns>Membership entry</returns>
        public async Task<Membership> FindUserByUsername(string username)
        {
            return (await context.Execute(new QueryProc<Membership>("aspnet_Membership_GetUserByName")
                .Param("@ApplicationName", options.ApplicationName)
                .Param("@UserName", username)
                .Param("@CurrentTimeUtc", DateTime.UtcNow)
                .Param("@UpdateLastActivity", true)
                .Map(reader => new Membership
                {
                    UserId = reader.Get<Guid>("UserId"),
                    UserName = username,
                    Email = reader.Get<string>("Email"),
                    PasswordQuestion = reader.Get<string>("PasswordQuestion"),
                    IsApproved = reader.Get<bool>("IsApproved"),
                    IsLockedOut = reader.Get<bool>("IsLockedOut"),
                    Comment = reader.Get<string>("Comment"),
                    CreateDate = reader.Get<DateTime>("CreateDate"),
                    LastLoginDate = reader.Get<DateTime>("LastLoginDate"),
                    LastActivityDate = reader.Get<DateTime>("LastActivityDate"),
                    LastPasswordChangedDate = reader.Get<DateTime>("LastPasswordChangedDate"),
                    LastLockoutDate = reader.Get<DateTime>("LastLockoutDate")
                })).ConfigureAwait(false)).SingleOrDefault();
        }

        /// <summary>Find password data for the Username</summary>
        /// <param name="username">Username</param>
        /// <returns>Membership Security</returns>
        public async Task<MembershipSecurity> GetUserPassword(string username)
        {
            return (await context.Execute(new QueryProc<MembershipSecurity>("aspnet_Membership_GetPasswordWithFormat")
                .Param("@ApplicationName", options.ApplicationName)
                .Param("@UserName", username)
                .Param("@UpdateLastLoginActivityDate", true)
                .Param("@CurrentTimeUtc", DateTime.UtcNow)
                .Map(reader => new MembershipSecurity
                {
                    Password = reader.Get<string>("0"),
                    PasswordFormat = reader.Get<int>("1"),
                    Salt = reader.Get<string>("2"),
                    FailedPasswordAttemptCount = reader.Get<int>("3"),
                    FailedPasswordAnswerAttemptCount = reader.Get<int>("4"),
                    IsApproved = reader.Get<bool>("5"),
                    LastLoginDate = reader.Get<DateTime>("6"),
                    LastActivityDate = reader.Get<DateTime>("7")
                })).ConfigureAwait(false)).SingleOrDefault();
        }

        /// <summary>Update user info based on attepted login</summary>
        /// <param name="userName">Username</param>
        /// <param name="membershipSecurity">Membership Security</param>
        /// <param name="isPasswordCorrect">Was the password attempt correct</param>
        public Task UpdateUserInfo(string userName, MembershipSecurity membershipSecurity, bool isPasswordCorrect)
        {
            return context.Execute(new QueryProc("aspnet_Membership_UpdateUserInfo")
                .Param("@ApplicationName", options.ApplicationName)
                .Param("@UserName", userName)
                .Param("@IsPasswordCorrect", isPasswordCorrect)
                .Param("@UpdateLastLoginActivityDate", true)
                .Param("@MaxInvalidPasswordAttempts", options.MaxInvalidPasswordAttempts)
                .Param("@PasswordAttemptWindow", options.PasswordAttemptWindow)
                .Param("@CurrentTimeUtc", DateTime.UtcNow)
                .Param("@LastLoginDate", isPasswordCorrect ? DateTime.UtcNow : membershipSecurity.LastLoginDate)
                .Param("@LastActivityDate", isPasswordCorrect ? DateTime.UtcNow : membershipSecurity.LastActivityDate)
            );
        }
    }
}