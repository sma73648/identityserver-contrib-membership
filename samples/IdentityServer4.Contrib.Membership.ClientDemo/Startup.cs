// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer4.Contrib.Membership.ClientDemo
{
    using System.IdentityModel.Tokens.Jwt;
    using IdentityServer4.Models;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authentication.OpenIdConnect;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddAuthentication(IdentityConstants.ApplicationScheme).AddOpenIdConnect(x =>
            {
                x.SignInScheme = IdentityServerConstants.DefaultCookieAuthenticationScheme;
                x.Authority = "http://localhost:5000";
                x.RequireHttpsMetadata = false;
                x.ClientId = "ServiceStack.SelfHost";
                x.ClientSecret = "F621F470-9731-4A25-80EF-67A6F7C5F4B8";
                x.ResponseType = "code id_token";
                x.GetClaimsFromUserInfoEndpoint = true;
                x.SaveTokens = true;
                x.Scope.Add("openid");
                x.Scope.Add("profile");
                x.Scope.Add("ServiceStack.SelfHost");
                x.Scope.Add("email");
                x.Scope.Add("offline_access");
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(o => o.LoginPath = new PathString("/login"));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();

            
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();


            app.UseMvcWithDefaultRoute();
        }
    }
}
