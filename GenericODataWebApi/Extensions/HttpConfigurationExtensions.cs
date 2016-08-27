using System.Collections;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.OData.Batch;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using System.Web.OData.Routing;
using System.Web.OData.Routing.Conventions;
using GenericODataWebApi.OData;
using Microsoft.OData.Edm;

namespace GenericODataWebApi
{
    public static class HttpConfigurationExtensions
    {
        public static void EnableRouteBasedODataControllerSelection(this HttpConfiguration config)
        {
            config.Services.Replace(typeof(IHttpControllerSelector), new RouteBasedODataControllerSelector(config));
        }

        public static void EnableOData(this HttpConfiguration config, ODataModelBuilder builder)
        {
            builder.SetupCustomModelAliasing();
            var edmModel = builder.GetEdmModel();
            var routingConventions = GetRoutingConventions(config, edmModel);

            config.MapODataServiceRoute(
                routeName: "ODataRoute", //todo: support multiple names
                routePrefix: null,
                model: edmModel,
                pathHandler: new DefaultODataPathHandler(),
                routingConventions: routingConventions,
                batchHandler: new DefaultODataBatchHandler(GlobalConfiguration.DefaultServer));
        }

        private static IEnumerable<IODataRoutingConvention> GetRoutingConventions(HttpConfiguration config, IEdmModel edmModel)
        {
            //todo: Replace existing conventions, where they have been directly overridden
            //todo: Still allow routing conventions to be modified!
            var routingConventions = ODataRoutingConventions.CreateDefaultWithAttributeRouting(config, edmModel);

            //Interesting note: Although the "GenericPropertyRoutingConvention" is added last, to allow direct implementations of property-getters to
            //be selected by the regular OData routing conventions, if you directly implement a property-getter, with the correct name, but fail to name
            //the "[FromODataUri]" parameter "key", the regular PropertyRoutingConvention will cause the following error!:
            // "No action was found on the controller '<name>' that matches the request."
            routingConventions.Add(new GenericPropertyRoutingConvention());

            //For some reason, if these aren't first, then the regular routing conventions will at some point return the entire entity set
            //when querying for a single item by key!
            routingConventions.Insert(0, new KeyRoutingConvention());
            routingConventions.Insert(0, new KeyDetectorRoutingConvention());

            return routingConventions;
        }

        private static void SetupCustomModelAliasing(this ODataModelBuilder builder)
        {
            var conventionBuilder = builder as ODataConventionModelBuilder;
            if (conventionBuilder != null)
            {
                //todo: factory pattern?
                var providers = new IOnModelCreatingProvider[] 
                {
                    new DefaultUserSpecifiedOnModelCreatingProvider(conventionBuilder),
                    new CustomAliasingOnModelCreatingProvider()
                };

                var aggregateProvider = new AggregateOnModelCreatingProvider(conventionBuilder, providers);
                conventionBuilder.OnModelCreating = aggregateProvider.GetOnModelCreating();
            }
        }
    }
}