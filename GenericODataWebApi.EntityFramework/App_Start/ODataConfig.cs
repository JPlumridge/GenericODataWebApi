using System;
using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Batch;
using System.Web.OData.Extensions;
using System.Web.OData.Routing;
using System.Web.OData.Routing.Conventions;
using GenericODataWebApi;

namespace GenericODataWebApi.EntityFramework
{
    public static class ODataConfig
    {
        public static void Register(HttpConfiguration config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

			// This allows the framework to select and activate a controller based on the route alone.
			// I.e using the entities registered below, a request to http://myservice/Users will active a controller with Type "UserDTO"
            config.EnableRouteBasedODataControllerSelection();

            var builder = new ODataConventionModelBuilder();

            /*
				This is where you set up your OData entity sets. For each entity set, you will
				have a seperate endpoint on your service. I.e. http://localhost/Users

				For more information, see: http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api

            builder.EntitySet<TitleDTO>("Titles");
            builder.EntitySet<UserDTO>("Users");

            */

            config.EnableOData(builder);
        }
    }
}