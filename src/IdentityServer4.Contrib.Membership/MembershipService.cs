// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer4.Contrib.Membership
{
    using System;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using Helpers;
    using Interfaces;

    /// <summary>Membership Service that performs some of the MembershipProvider read logic</summary>
    public class MembershipService : IMembershipService
    {
        private readonly IMembershipRepository membershipRepository;
        private readonly IMembershipPasswordHasher membershipPasswordHasher;

        /// <summary>Constructor</summary>
        /// <param name="membershipRepository">Membership Repository</param>
        /// <param name="membershipPasswordHasher">Membership Password Hasher</param>
        public MembershipService(IMembershipRepository membershipRepository, IMembershipPasswordHasher membershipPasswordHasher)
        {
            this.membershipRepository = membershipRepository.ThrowIfNull(nameof(membershipRepository));
            this.membershipPasswordHasher = membershipPasswordHasher.ThrowIfNull(nameof(membershipPasswordHasher));
        }

        /// <summary>Gets a User by their Unique Identifier</summary>
        /// <param name="userId">User Id</param>
        /// <returns>Membership User</returns>
        public async Task<MembershipUser> GetUserAsync(Guid userId)
        {
            var user = await membershipRepository.FindUserById(userId)
                                                 .ConfigureAwait(false);

            if (user == null) return null;

            return new MembershipUser
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                IsLockedOut = user.IsLockedOut,
                AccountCreated = user.CreateDate,
                LastActivity = user.LastActivityDate,
                PasswordChanged = user.LastPasswordChangedDate
            };
        }

        /// <summary>Gets a User by their Username</summary>
        /// <param name="username">Username</param>
        /// <returns>Membership User</returns>
        public async Task<MembershipUser> GetUserAsync(string username)
        {
            var user = await membershipRepository.FindUserByUsername(username)
                                                 .ConfigureAwait(false);

            if (user == null) return null;

            return new MembershipUser
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                IsLockedOut = user.IsLockedOut,
                AccountCreated = user.CreateDate,
                LastActivity = user.LastActivityDate,
                PasswordChanged = user.LastPasswordChangedDate
            };
        }

        /// <summary>Validates the given password is valid for a user</summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>True if valid, False if not</returns>
        public async Task<bool> ValidateUser(string username, string password)
        {
            // Get the known user password data from the membership repository
            var userSecurity = await membershipRepository.GetUserPassword(username)
                                                         .ConfigureAwait(false);
            if (userSecurity == null) return false;

            // Encrypt the password given using the same encryption data stored against the user
            var encryptedPassword = membershipPasswordHasher.EncryptPassword(password, userSecurity.PasswordFormat, userSecurity.Salt);

            // The Encrypted password should match the password held in the datastore
            var isPasswordCorrect = userSecurity.Password == encryptedPassword;

            // Update our user with any failed password attempts, resets etc.
            if (!isPasswordCorrect ||
                userSecurity.FailedPasswordAttemptCount != 0 ||
                userSecurity.FailedPasswordAnswerAttemptCount != 0)
            {
                await membershipRepository.UpdateUserInfo(username, userSecurity, isPasswordCorrect)
                                          .ConfigureAwait(false);
            }

            return isPasswordCorrect;
        }

        /// <summary>Updates the password for the given username</summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task UpdatePassword(string username, string password)
        {
            // This is how asp.net membership generates the salt
            var rng = RandomNumberGenerator.Create();
            var buf = new byte[16];
            rng.GetBytes(buf);
            var salt = Convert.ToBase64String(buf);

            var encryptedPassword = membershipPasswordHasher.EncryptPassword(password, 1, salt);

            await membershipRepository.UpdatePassword(username, encryptedPassword, salt, 1);
        }
    }
}
