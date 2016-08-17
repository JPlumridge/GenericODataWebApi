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

            var routingConventions = ODataRoutingConventions.CreateDefaultWithAttributeRouting(config, edmModel);
            routingConventions.Insert(0, new PropertyODataRoutingConvention());
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