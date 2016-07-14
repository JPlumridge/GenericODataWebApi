using System.Data.Entity;
using Microsoft.Practices.Unity;
using System.Web.Http;
using AutoMapper;
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
				Here, you need to do the following, where "MyEntities" is the type of your EF DbContext

            container.RegisterType<DbContext, MyEntities>();

			*/

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}