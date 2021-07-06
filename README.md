## Optimizely .NET Core Preview

This repository is a preview providing early access to the latest Optimizely (formerly Episerver) product packages targeting .NET 5. Please use GitHub issues to provide feedback regarding any specific issues you have encountered when upgrading your add-ons or projects.

---

## The solution

Use your user directory on Windows, or add Everyone to the folder where you created the repository. The Alloy and Quicksilver sample sites are both using localdb, so you need to have the correct permissions.

`NOTE: Quicksilver has a default username and password of admin@example.com / Episerver123!`

---

## NuGet package location

This preview repository has a nuget.config with the location to the packages.  If you need to add your own nuget.config or update package sources, use the following location.
  1.  https://pkgs.dev.azure.com/EpiserverEngineering/netCore/_packaging/beta-program/nuget/v3/index.json

---

## Template installation

```
dotnet new -i EPiServer.Net.Templates::1.0.0-pre-020034 --nuget-source https://pkgs.dev.azure.com/EpiserverEngineering/netCore/_packaging/beta-program/nuget/v3/index.json --force
```
---

## CLI installation

```
dotnet tool install EPiServer.Net.Cli --global --add-source https://pkgs.dev.azure.com/EpiserverEngineering/netCore/_packaging/beta-program/nuget/v3/index.json --version 1.0.0-pre-020034
```
---

## Create empty CMS site

```
dotnet new epicmsempty --name ProjectName
cd projectname
dotnet-episerver create-cms-database ProjectName.csproj -S . -E 

```
### Notes

-S stands for the server you want to connect to.  Do not use . if you dont have a local sql server installed.  You could also use MahcineName\SQLEXRESSS or (LocalDb)\MSSqlLocalDB instead of ".".
Right now there is no command to create the admin user, we plan to add in the future.  You can check Quicksilver\EPiServer.Reference.Commerce.Site\Infrastructure\UsersInstaller.cs if you want to automate in the short term.

---

## Create empty Commerce site

```
dotnet new epicommerceempty --name ProjectName
cd projectname
dotnet-episerver create-cms-database ProjectName.csproj -S . -E
dotnet-episerver create-commerce-database ProjectName.csproj -S . -E --reuse-cms-user
```
### Notes

-S stands for the server you want to connect to.  Do not use . if you dont have a local sql server installed.  You could also use MahcineName\SQLEXRESSS or (LocalDb)\MSSqlLocalDB instead of ".".
Right now there is no command to create the admin user, we plan to add in the future.  You can check Quicksilver\EPiServer.Reference.Commerce.Site\Infrastructure\UsersInstaller.cs if you want to automate in the short term.

---


## Configuration

Most of the configuration has been moved to options classes. The options classes can be configured through code or the appsettings.json configuration file. For option classes to be automatically configured from `appsettings.json`, use the `EPiServer.ServiceLocation.OptionsAttribute`. There is a configuration section which maps to the leaf node in the JSON.

To utilize legacy configuration sections you can install the `EPiServer.Cms.AspNetCore.Migration` package. This is available to ease migration, however we encourage to update the use options or `appsettings.json` if possible.

---

## Startup extensibility
### Program.cs
EPiServer will by default use the built-in Dependency Injection framework (DI) in .NET 5. To connect the DI framework with EPiServer you need to call extension method `IHostBuilder.ConfigureCmsDefault()` in Program.cs. <br/>
To configure the application (including EPiServer) to use another DI framework you should call the extension method `IHostBuilder.UseServiceProviderFactory`. The example below shows how to configure the application to use Autofac:

```
host.UseServiceProviderFactory(context => new  ServiceLocatorProviderFactoryFacade<ContainerBuilder>(context,
    new AutofacServiceProviderFactory()));
```

### Startup.cs
There are some added extensibility points when interacting with the Startup class.
  1.  `services.AddCms();` - This configures than CMS and needs to be called to function properly.
  2.  `endpoints.MapContent();` - This registers EPiServer content routing with the endpoint routing feature.
  3.  `IEndpointRoutingExtension` - Access to the `IEndpointRouteBuilder` to register routes. Convience method `services.AddEndpointRoutingExtension<T>()`
  4.  `IStartupFilter` - Access to IApplicationBuilder if you need to register middleware for instance.  Convience method `services.AddStartupFilter<T>()`
  5.  `IBlockingFirstRequestInitializer` - Use this if you need to do something before the first request
  6.  `IRedirectingFirstRequestInitializer` - Use this if you need to redirect to a page until some type of initialization takes place.

---

## Compiled views for shell modules

For add-on developers, we have added a default location expander that will look for compiled views in a certain location or based on configuration value.
  1.  /{ShellModuleName}/Views/
  2.  The folder defined in the module.config viewFolder attribute on module element.

---

## Search & Navigation (formerly Find) preview

To use the updated Find packages you will need to configure the .NET 5 version in the Startup file.

```
services.Configure<FindUIOptions>(x => x.ClientSideResourceBaseUrl = "https://stage.dl.episerver.net/$version$/");
```

---

## Preview of documentation

For a preview of the documentation for CMS 12, Commerce 14, and Search & Navigation 14, including Breaking changes, see the [.NET 5.0 preview documentation](https://world.episerver.com/documentation/developer-guides/optimizely-platform/-net-core-preview/ ".NET 5.0 preview documentation") on Optimizely World.
