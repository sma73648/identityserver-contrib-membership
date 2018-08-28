// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Membership.Demo
{
    using System.Collections.Generic;
    using System.Security.Cryptography.X509Certificates;
    using Core;
    using Core.Configuration;
    using Core.Models;
    using Owin;

    class Startup
    {
        private const string MembershipDb = "Membership";
        private const string MembershipApplicationName = "Test";

        public void Configuration(IAppBuilder app)
        {
            MembershipTestData.SetUp(MembershipDb, MembershipApplicationName);

            var factory = new IdentityServerServiceFactory()
                .UseInMemoryClients(Clients.Get())
                .UseInMemoryScopes(Scopes.Get());

            factory.UseMembershipService(
                new MembershipOptions
                {
                    ConnectionString = "Data Source=localhost;Initial Catalog=Membership;Integrated Security=True",
                    ApplicationName = "Test"
                });

            var options = new IdentityServerOptions
            {
                SigningCertificate = LoadCertificate(),

                Factory = factory,

                RequireSsl = false
            };

            app.UseIdentityServer(options);
        }

        private X509Certificate2 LoadCertificate()
        {
            using (var stream = typeof(Startup).Assembly.GetManifestResourceStream("IdentityServer3.Contrib.Membership.Demo.idsrv3test.pfx"))
            {
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                return new X509Certificate2(bytes, "idsrv3test");
            }
        }
    }

    class Clients
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

                    AccessTokenType = AccessTokenType.Jwt,

                    Flow = Flows.Hybrid,

                    ClientSecrets = new List<Secret>
                    {
                        new Secret("F621F470-9731-4A25-80EF-67A6F7C5F4B8".Sha256())
                    },

                    AllowAccessToAllScopes = true,

                    RedirectUris = new List<string>
                    {
                        "http://localhost:5001/auth/IdentityServer"
                    },

                    RequireConsent = false
                }
            };
        }
    }

    class Scopes
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
                        new ScopeClaim(Constants.ClaimTypes.Subject),
                        new ScopeClaim(Constants.ClaimTypes.PreferredUserName)
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
