// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer4.Contrib.Membership.IdsvrDemo.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using IdentityModel;
    using Microsoft.AspNetCore.Mvc;
    using Model;
    using Services;

    using IdentityServer4.Contrib.Membership.Interfaces;

    public class LoginController : Controller
    {
        private readonly IMembershipService membershipService;
        private readonly IClaimsService claimsService;
        private readonly SignInInteraction signInInteraction;

        public LoginController(IMembershipService membershipService, IClaimsService claimsService, SignInInteraction signInInteraction)
        {
            this.membershipService = membershipService;
            this.claimsService = claimsService;
            this.signInInteraction = signInInteraction;
        }

        [HttpGet(Constants.RoutePaths.Login, Name = "Login")]
        public async Task<IActionResult> Index(string id)
        {
            var vm = new LoginViewModel();
            if (id != null)
            {
                var request = await signInInteraction.GetRequestAsync(id);
                if (request != null)
                {
                    vm.Username = request.LoginHint;
                    vm.SignInId = id;
                }
            }
            return View(vm);
        }

        [HttpPost(Constants.RoutePaths.Login)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginInputModel model)
        {
            if (ModelState.IsValid)
            {
                if (await membershipService.ValidateUser(model.Username, model.Password)
                                           .ConfigureAwait(false))
                {
                    var claims = await claimsService.GetClaimsFromAccount(model.Username)
                                                    .ConfigureAwait(false);
                    var claimsIdentity = new ClaimsIdentity(claims, "password", JwtClaimTypes.Name, JwtClaimTypes.Role);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.Authentication.SignInAsync(Constants.PrimaryAuthenticationType, claimsPrincipal);

                    if (model.SignInId != null)
                    {
                        return new ActionResults.SignInResult(model.SignInId);
                    }

                    return Redirect("~/");
                }
            }

            var vm = new LoginViewModel(model);
            return View(vm);
        }
    }
}
