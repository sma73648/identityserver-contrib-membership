// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer4.Contrib.Membership.IdsvrDemo
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using IdentityModel;
    using IdentityServer4.Models;

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services
                .AddDeveloperIdentityServer()
                .AddInMemoryClients(Clients.Get())
                .AddInMemoryScopes(Scopes.Get())
                .AddMembershipService(new MembershipOptions
                {
                    ConnectionString = "Data Source=localhost;Initial Catalog=Membership;Integrated Security=True",
                    ApplicationName = "Test" 
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();  
                      
            app.UseIdentityServer();

            app.UseMvcWithDefaultRoute();
        }

        static class Clients
        {
            public static List<Client> Get()
            {
                return new List<Client>
                {
                    new Client
                    {
                        ClientName = "ServiceStack.SelfHost",
                        ClientId = "ServiceStack.SelfHost",
                        Enabled = true,

                        AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,

                        AccessTokenType = AccessTokenType.Jwt,

                        ClientSecrets = new List<Secret>
                        {
                            new Secret("F621F470-9731-4A25-80EF-67A6F7C5F4B8".Sha256())
                        },

                        AllowAccessToAllScopes = true,

                        RedirectUris = new List<string>
                        {
                            "http://localhost:5001/signin-oidc"
                        },

                        RequireConsent = false
                    }
                };
            }
        }

        static class Scopes
        {
            public static List<Scope> Get()
            {
                return new List<Scope>(StandardScopes.All)
            {
                StandardScopes.OfflineAccess,
                new Scope
                {
                    Enabled = true,
                    Name = "ServiceStack.SelfHost",
                    Type = ScopeType.Identity,
                    Claims = new List<ScopeClaim>
                    {
                        new ScopeClaim(JwtClaimTypes.Subject),
                        new ScopeClaim(JwtClaimTypes.Name)
                    },
                    ScopeSecrets = new List<Secret>
                    {
                        new Secret("F621F470-9731-4A25-80EF-67A6F7C5F4B8".Sha256())
                    }
                }
            };
            }
        }
    }
}
