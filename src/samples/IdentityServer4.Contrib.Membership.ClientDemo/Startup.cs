// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer4.Contrib.Membership.ClientDemo
{
    using System.IdentityModel.Tokens.Jwt;
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

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie("Cookies")
                .AddOpenIdConnect(options =>
                {
                    options.SignInScheme = "Cookies";

                    // Authority must be handled via https only and RequireHttpsMetadata must be set to true on production
                    options.RequireHttpsMetadata = false;
                    options.Authority = "http://localhost:5000";
                    options.ClientId = "ServiceStack.SelfHost";
                    options.ClientSecret = "F621F470-9731-4A25-80EF-67A6F7C5F4B8";
                    options.ResponseType = "code id_token";
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.SaveTokens = true;

                    var scopes = new[] { "openid", "profile", "ServiceStack.SelfHost", "email", "offline_access" };
                    foreach (var scope in scopes)
                    {
                        options.Scope.Add(scope);
                    }
                });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.UseAuthentication();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            app.UseMvcWithDefaultRoute();
        }
    }
}