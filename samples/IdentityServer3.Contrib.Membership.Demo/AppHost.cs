// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Membership.Demo
{
    using System.IO;
    using Funq;
    using global::ServiceStack;
    using global::ServiceStack.Razor;
    using ServiceStack.Authentication.IdentityServer;
    using ServiceStack.Authentication.IdentityServer.Extensions;

    /// <summary>Service Stack App Host</summary>
    class AppHost : AppSelfHostBase
    {
        private readonly string serviceUrl;

        public AppHost(string serviceUrl)
            : base("ServiceStack.SelfHost", typeof (AppHost).Assembly)
        {
            this.serviceUrl = serviceUrl;
        }

        public override void Configure(Container container)
        {
            this.Plugins.Add(new RazorFormat());
            SetConfig(new HostConfig
            {
#if DEBUG
                DebugMode = true,
                WebHostPhysicalPath = Path.GetFullPath(Path.Combine("~".MapServerPath(), "..", "..")),
#endif
                WebHostUrl = serviceUrl
            });

            AppSettings.SetUserAuthProvider()
                       .SetAuthRealm("http://localhost:5000/")
                       .SetClientId("ServiceStack.SelfHost")
                       .SetClientSecret("F621F470-9731-4A25-80EF-67A6F7C5F4B8")
                       .SetScopes("openid profile ServiceStack.SelfHost email offline_access");

            this.Plugins.Add(new IdentityServerAuthFeature());
        }
    }
}
