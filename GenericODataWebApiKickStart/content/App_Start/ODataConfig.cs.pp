using System;
using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using System.Web.OData.Routing;
using System.Web.OData.Routing.Conventions;
using GenericODataWebApi;

namespace $rootnamespace$
{
    public static class ODataConfig
    {
        public static void Register(HttpConfiguration config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            config.EnableGenericODataController();

            var builder = new ODataConventionModelBuilder();

            /*
				This is where you set up your OData entity sets. For each entity set, you will
				have a seperate endpoint on your service. I.e. http://localhost/Users

            builder.EntitySet<TitleDTO>("Titles");
            builder.EntitySet<UserDTO>("Users");

            */

            var routingConventions = ODataRoutingConventions.CreateDefault();
            routingConventions.Insert(0, new PropertyODataRoutingConvention());

            config.MapODataServiceRoute(
                routeName: "ODataRoute",
                routePrefix: null,
                model: builder.GetEdmModel(),
                pathHandler: new DefaultODataPathHandler(),
                routingConventions: routingConventions);
        }
    }
}