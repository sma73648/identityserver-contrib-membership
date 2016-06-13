# IdentityServer3.Contrib.Membership

## ASP.NET 2.0 Membership Database as Identity Server User Store
Identity Server is a framework and doesn't provide implementations of user data sources out of the box.
If you have an existing ASP.NET 2.0 Membership Database containing user data for existing systems then you can install the following package:

```powershell
PM> Install-Package IdentityServer3.Contrib.Membership
```

To add the plugin, add the following to the OWIN startup class of your IdentityServer instance:
```csharp
public void Configuration(IAppBuilder app)
{
    var factory = new IdentityServerServiceFactory();        
    ...
    factory.UseMembershipService(
        new MembershipOptions
        {
                ConnectionString = "...",   // Membership database connection string
                ApplicationName = "..."     // Membership Application Name
        });        
    ...
}
```

This will validate user logins and passwords against an existing database.  No support is provided for maintaining users and it is not recommended that you use this for a new implementation. 
IdentityServer provides a [plugin](https://github.com/IdentityServer/IdentityServer3.AspNetIdentity) that supports [ASP.NET Identity](http://www.asp.net/identity).

## Overview
Project IdentityServer3.Membership is an implementation of a IUserService that authenticates against an existing ASP.NET 2.0 Membership database without having to use the SqlMembershipProvider.
To add this to the OWIN startup class of an IdentityServer instance add the following to the Configuration method:

```csharp
public void Configuration(IAppBuilder app)
{
    var factory = new IdentityServerServiceFactory();
    
    ...
    factory.UseMembershipService(
        new MembershipOptions
        {
            ConnectionString = "Data Source=localhost;Initial Catalog=Membership;Integrated Security=True",
            ApplicationName = "/"
        });
        
    ...
}
```

## IdentityServer3.Contrib.Membership.Demo
A demo project that authenticates a [ServiceStack](https://servicestack.net/) razor-based Client App using [IdentityServer](https://identityserver.github.io/)
using an ASP.NET Membership Database for User data.

## Overview
This demo project bring in the various Identity Server and Service Stack plugins available in this Solution, namely:
* IdentityServer3.Contrib.Membership - An IdentityServer plugin that stores user data
* IdentityServer3.Contrib.ServiceStack - An IdentityServer plugin that supports impersonation authentication of a ServiceStack instance using IdentityServer
* ServiceStack.IdentityServerAuthProvider - A ServiceStack AuthProvider that authenticates a user against an IdentityServer instance

When the project starts, you should be presented with a simple ServiceStack web app with a link that redirects to a secure service in ServiceStack. When you select the link you should be redirected to the IdentityServer instance that prompts you for login details.  Login using username "test@test.com" with password "password123".  You should then be redirected back to the ServiceStack web app and have access to the secure service (with Authenticate attribute) which displays the secure message.

### Prerequisites
* Create a SQL Server database called "Membership" using aspnet_regsql.exe (update app.config with the correct connectionString).  See below for instructions.

#### Creating an empty ASP.NET 2.0 Membership database
To create an empty ASP.NET 2.0 Membership database, run the following command:
    `C:\Windows\Microsoft.NET\Framework\v2.0.50727\aspnet_regsql.exe`

When the wizard opens, select next then "Configure SQL Server for application services" then next again. Select the Server instance on which the database will run and give the Database name "Membership" then continue.

The sample code can be viewed [here](/samples/IdentityServer3.Contrib.Membership.Demo)
