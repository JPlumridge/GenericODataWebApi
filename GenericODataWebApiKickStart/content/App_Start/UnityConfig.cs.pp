using System.Data.Entity;
using Microsoft.Practices.Unity;
using System.Web.Http;
using Unity.WebApi;
using GenericODataWebApi;

namespace $rootnamespace$
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

			/*
				Here, you need to do the following, where "MyDataProvider" is the type of your Data provider
				Use the package "GenericODataWebApi.EntityFramework" for a ready to go EF provider

            container.RegisterType<IODataProvider, MyDataProvider>();

			*/

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}