using Microsoft.Practices.Unity;
using System.Web.Http;
using Unity.WebApi;
using GenericODataWebApi;

namespace GenericODataWebApi.EntityFramework
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

			/*
				Here, you need to do the following, where "MyODataProvider" is the type of your Data provider.
				You have two options:

				1. Create your own class that implements IODataProvider
				2. Use the package "GenericODataWebApi.EntityFramework" for a ready to go EntityFramework provider.

			container.RegisterType(typeof(IODataProvider<>), typeof(MyODataProvider<>));

			*/

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}