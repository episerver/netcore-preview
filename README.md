<a href="https://github.com/episerver/netcore-preview"><img src="http://ux.episerver.com/images/logo.png" title="netcore-preview" alt="netcore-preview"></a>

## Netcore Preview

This preview repository is early access to the latest Episerver packages targeting .NET 5. Here we will collect feedback through github issues regarding specific issues when upgrading your addons or projects.

---

## The Solution

Please use your user directory on windows or add everyone to the folder where you created the repo.  Alloy and Quicksilver are both using localdb so you need to have the correct permissions.

`Quicksilver has a default username and password of admin@example.com / Episerver123!`

---

## Nuget Package Location

This preview repository has a nuget.config with the location to the packages.  If you need to add your own nuget.config or update package sources, please use the following location.
  1.  https://pkgs.dev.azure.com/EpiserverEngineering/netCore/_packaging/beta-program/nuget/v3/index.json

---

## Template Installation

```
dotnet new -i EPiServer.Net.Templates::1.0.0-inte-017362 --nuget-source https://pkgs.dev.azure.com/EpiserverEngineering/netCore/_packaging/beta-program/nuget/v3/index.json --force
```
---

## CLI Installation

```
dotnet tool install EPiServer.Net.Cli --global --add-source https://pkgs.dev.azure.com/EpiserverEngineering/netCore/_packaging/beta-program/nuget/v3/index.json --version 1.0.0-inte-017362
```
---

## Create empty cms site

```
dotnet new epicmsempty --name ProjectName
cd projectname
dotnet-episerver create-cms-database ProjectName.csproj -S . -E 

```
### Notes

-S stands for the server you want to connect to.  Do not use . if you dont have a local sql server installed.  You could also use MahcineName\SQLEXRESSS or (LocalDb)\MSSqlLocalDB instead of ".".
Right now there is no command to create the admin user, we plan to add in the future.  You can check Quicksilver\EPiServer.Reference.Commerce.Site\Infrastructure\UsersInstaller.cs if you want to automate in the short term.

---

## Create empty commerce site

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

## Create alloy site

```
dotnet new epicmsalloy --name ProjectName
cd projectname
dotnet-episerver create-cms-database ProjectName.csproj -S . -E
```

### Notes

-S stands for the server you want to connect to.  Do not use . if you dont have a local sql server installed.  You could also use MahcineName\SQLEXRESSS or (LocalDb)\MSSqlLocalDB instead of ".".
Alloy has a middleware to create the administration user.

---

## Configuration

Most of the configuration has been moved to options classes.  The options classes can be configured through code or the appsettings.json configuration file.  For option classes to be automatically configured from `appsettings.json`, please use the `EPiServer.ServiceLocation.OptionsAttribute`.  There is a configuration section which maps to the leaf node in the JSON.

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

## Compiled Views for Shell Modules

For addon developers, we have added a default location expander that will look for compiled views in a certain location or based on configuration value.
  1.  /{ShellModuleName}/Views/
  2.  The folder defined in the module.config viewFolder attribute on module element.

---

## Preview links to documentation

### CMS

* [CMS 12 Breaking changes](https://world.episerver.com/externalContentView/2f46e48e-19d5-4735-a0fa-f9b193a78eb7 "CMS 12 Breaking changes")
* [Configuration](https://world.episerver.com/externalContentView/91e3ad6f-ec40-44c4-a667-7d48e2f1c2f0 "Configuration")
* [Dependency injection](https://world.episerver.com/externalContentView/a798288a-90af-44ff-b495-68e827403903 "Dependency injection")
* [File Providers](https://world.episerver.com/externalContentView/c10509ac-40b4-4c0e-92d5-016b4e37081b "File Providers")
* [Logging](https://world.episerver.com/externalContentView/ac48d781-f9f6-4f16-8677-8281bacdaffa "Logging")
* [Routing](https://world.episerver.com/externalContentView/968f52c2-8a0f-4111-a34a-d51450d62b1e "Routing")
* [Security](https://world.episerver.com/externalContentView/bf63f0a1-da67-4a0c-8b71-d753099956d0 "Security")
* [Upgrade assistant - overview](https://world.episerver.com/externalContentView/01ad2880-18c2-4898-90d0-9fa99a6fdbe1 "Upgrade assistant - overview")
* [Upgrade assistant - installation and running](https://world.episerver.com/externalContentView/f5838f07-fc03-464a-9bff-724272e6bf1e "Upgrade assistant - installation and running")

### Commerce

