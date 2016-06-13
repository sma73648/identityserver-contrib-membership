// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Membership.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using DataAccess.Entities;

    public interface IMembershipRepository
    {
        Task<Membership> FindUserById(Guid id);

        Task<Membership> FindUserByUsername(string username);

        Task<MembershipSecurity> GetUserPassword(string username);

        Task UpdateUserInfo(string userName, MembershipSecurity membershipSecurity, bool isPasswordCorrect);
    }
}
