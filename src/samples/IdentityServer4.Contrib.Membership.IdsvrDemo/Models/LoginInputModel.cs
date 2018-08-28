// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer4.Contrib.Membership.IdsvrDemo.Models
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Login Input Model - taken from the original IdentityServer4 <a href="https://github.com/IdentityServer/IdentityServer4.Samples/">Samples.</a>
    /// </summary>
    public class LoginInputModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public bool RememberLogin { get; set; }

        public string ReturnUrl { get; set; }
    }
}
