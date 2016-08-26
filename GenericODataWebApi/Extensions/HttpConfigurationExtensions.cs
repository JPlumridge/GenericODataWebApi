using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.OData.Batch;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using System.Web.OData.Routing;
using System.Web.OData.Routing.Conventions;

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
            var edmModel = builder.GetEdmModel();

            //todo: Replace existing conventions, where they have been directly overridden
            var routingConventions = ODataRoutingConventions.CreateDefaultWithAttributeRouting(config, edmModel);

            //Interesting note: Although the "PropertyODataRoutingConvention" is added last, to allow direct implementations of property-getters to
            //be selected by the regular OData routing conventions, if you directly implement a property-getter, with the correct name, but fail to name
            //the "[FromODataUri]" parameter "key", the regular PropertyRoutingConvention will cause the following error!:
            // "No action was found on the controller '<name>' that matches the request."
            routingConventions.Add(new PropertyODataRoutingConvention());

            //For some reason, if these aren't first, then the regular routing conventions will at some point return the entire entity set
            //when querying for a single item by key!
            routingConventions.Insert(0, new KeyRoutingConvention());
            routingConventions.Insert(0, new KeyDetectorRoutingConvention());

            config.MapODataServiceRoute(
                routeName: "ODataRoute", //todo: support multiple names
                routePrefix: null,
                model: edmModel,
                pathHandler: new DefaultODataPathHandler(),
                routingConventions: routingConventions,
                batchHandler: new DefaultODataBatchHandler(GlobalConfiguration.DefaultServer));
        }
    }
}