# GenericODataWebApi
Provides controllers that provide all OData functions for an entity set

Available on NuGet at https://www.nuget.org/packages/GenericODataWebApi
##Setup
In order to enable OData using this package, you need to do a few things:

1. Setup your OData model as you normally would (see http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api)
2. Do one of:
	* Enable automatic controller selection (recommended)
	* Create a controller derived from the provided controllers
3. Enable OData using extension method
4. Register your data provider with Unity

##Example
The NuGet package adds the file ``ODataConfig.cs`` which can be configured:
```C#
config.EnableRouteBasedODataControllerSelection();

//Standard OData model setup here
var builder = new ODataConventionModelBuilder();
builder.EntitySet<Dinosaur>("Dinosaurs");
builder.EntitySet<Location>("Locations");

config.EnableOData(builder);
```
The package aslo adds the file ``UnityConfig.cs`` in which an ``IODataProvider`` must be registered:
```C#
container.RegisterType(typeof(IODataProvider<>), typeof(MyODataProvider<>));
```
In the above, ``MyODataProvider.cs`` is a type you create that implements ``IODataProvider``
#Entity Framework
This package provides a ready-to-go EntityFramework implementation of the ``IODataProvider``

Available on NuGet at https://www.nuget.org/packages/GenericODataWebApi.EntityFramework
##Setup
Setup your project as described in the examples above, but when it comes to ``UnityConfig.cs`` simply do the following, where ``JurassicEntities`` is the name of your ``DbContext``
```C#
container.RegisterEntityFrameworkOData<JurassicEntities>();
```
#Coming in future updates
* MVC6 support
* Generic IQueryable dataprovider, possibly something like this:
```C#
container.RegisterIQueryableODataProvider(MyQueryable);
```
* Dependency on Unity to be removed?
