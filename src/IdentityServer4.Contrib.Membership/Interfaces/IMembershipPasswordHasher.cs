// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer4.Contrib.Membership.Interfaces
{
    /// <summary>Membership Password Encrypt / Hasher</summary>
    public interface IMembershipPasswordHasher
    {
        /// <summary>Encrypts a password using the given format and salt</summary>
        /// <param name="password">The password to encrypt / hash</param>
        /// <param name="passwordFormat">The format to use 0 = clear, 1 = encrypt, 2 = hash</param>
        /// <param name="salt">The salt to apply to the password</param>
        /// <returns>The encrypted / hashed password</returns>
        string EncryptPassword(string password, int passwordFormat, string salt);
    }
}
