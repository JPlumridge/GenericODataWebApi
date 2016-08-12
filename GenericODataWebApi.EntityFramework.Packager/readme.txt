Getting started
-----------------------

The Controllers provided by the package GenericODataWebApi depend on having an IODataProvider

Add the following to your UnityConfig.cs, in order to use the data provider in this package,
where "MyEntities" is the type of your EF DbContext:

	container.RegisterEntityFrameworkOData<MyEntities>();


