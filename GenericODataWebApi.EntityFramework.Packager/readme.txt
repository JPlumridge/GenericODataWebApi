Getting started with Generic Odata Entity Framework
---------------------------------------------------

The Controllers provided by the package GenericODataWebApi depend on having an IODataProvider

Add the following to your UnityConfig.cs, in order to use the data provider in this package,
where "MyEntities" is the type of your EF DbContext:

	container.RegisterEntityFrameworkOData<MyEntities>();


Getting started with Generic OData
----------------------------------

In order to enable the generic OData controller, the following must be added to WebApiConfig.cs:

public static void Register(HttpConfiguration config)
{
	if (config == null) throw new ArgumentNullException("config");
	Config = config;

	AddRoutes();
	InitialiseLogger();
	AddUnhandledExceptionFilter();
	AddBadRequestFilter();
	AddThrottle();
	AddCircuitBreaker();
	AddMonitoring();

	ODataConfig.Register(config);						// <----- Add this line
}

Please open the following files, and review the instructions within:

ODataConfig.cs
UnityConfig.cs

Note: If you want to use any attribute filters on the controller, you can add them to global filters


