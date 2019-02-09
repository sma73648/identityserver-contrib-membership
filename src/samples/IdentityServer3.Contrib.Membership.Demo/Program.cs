// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Membership.Demo
{
    using System;
    using System.Diagnostics;
    using global::ServiceStack.Text;
    using Microsoft.Owin.Hosting;
    using Serilog;

    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo
                .Console(outputTemplate: "{Timestamp:HH:mm} [{Level}] ({Name:l}){NewLine} {Message}{NewLine}{Exception}")
                .CreateLogger();

            IDisposable webApp = null;

            try
            {
                webApp = WebApp.Start<Startup>("http://localhost:5000");

                const string hostUrl = "http://localhost:5001/";
                new AppHost(hostUrl).Init().Start("http://*:5001/");
                $"ServiceStack Self Host with Razor listening at {hostUrl} ".Print();
                Process.Start(hostUrl);

                Console.WriteLine("Identity Server running....");
                Console.ReadLine();
            }
            finally
            {
                webApp?.Dispose();
            }
        }
    }
}
