# GenericODataWebApi
Provides controllers that provide all OData functions for an entity set, and automatic integration into the WebAPI pipeline

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
Here, you can either use a built in ``IODataProvider``, or create your own.
To register an in-memory ``IQueryable`` as the data source for an entity set, you can use the following extension method to register it with the built in ``QueryableDataProvider``:
```C#
container.RegisterQueryableODataSource(MyDinosaurQueryable);
```
Note that as the ``IQueryable`` interface does not support adding/removing of items, neither does the `QueryableDataProvider`
To provide your own custom data source, create a class that implements ``IODataProvider``
#Entity Framework
This package provides a ready-to-go EntityFramework implementation of the ``IODataProvider``

Available on NuGet at https://www.nuget.org/packages/GenericODataWebApi.EntityFramework
##Setup
Setup your project as described in the examples above, but when it comes to ``UnityConfig.cs`` simply do the following, where ``JurassicEntities`` is the name of your ``DbContext``
```C#
container.RegisterEntityFrameworkOData<JurassicEntities>();
```
##Keys
In the case that the Key of your OData entity is not the same as your Entity Framework primary key(s), you may provide a custom implementation of the ``IKeyLocatorStrategy<T>`` interface.
For example, suppose that we have an entity type ``Dinosaur``, whos Entity Framework primary key is an int called ``Id``, but for which we want to specify the OData key another property, a string called ``Name``.
We can create a custom key location strategy:

```C#
public class DinosaurKeyLocationStrategy : IKeyLocatorStrategy<Dinosaur>
{
	private DbContext db;
	public DinosaurKeyLocationStrategy(DbContext dbContext)
	{
	    this.db = dbContext;
	}
	
	public async Task<Dinosaur> FindByKey(IKeyProvider keyProvider)
	{
	    var nameKey = (string)(keyProvider.GetKeys().Single().Value);
	    return await db.Set<Dinosaur>().SingleAsync(d => d.Name == nameKey);
	}
}
```

Then, in UnityConfig.cs, we can register the key location strategy for this particular entity like this:

```C#
container.RegisterType<IKeyLocatorStrategy<Dinosaur>, DinosaurKeyLocationStrategy>();
```

#Coming in future updates
* MVC6 support
* Dependency on Unity to be removed?
