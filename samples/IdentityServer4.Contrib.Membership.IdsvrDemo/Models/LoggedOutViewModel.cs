// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer4.Contrib.Membership.IdsvrDemo.Models
{
    /// <summary>
    /// Logged Out View Model - taken from the original IdentityServer4 <a href="https://github.com/IdentityServer/IdentityServer4.Samples/">Samples.</a>
    /// </summary>
    public class LoggedOutViewModel
    {
        public string PostLogoutRedirectUri { get; set; }

        public string ClientName { get; set; }

        public string SignOutIframeUrl { get; set; }
    }
}
