using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace GenericODataWebApi
{
    public static class HttpConfigurationExtensions
    {
        public static void EnableRouteBasedODataControllerSelection(this HttpConfiguration config)
        {
            config.Services.Replace(typeof(IHttpControllerSelector), new RouteBasedODataControllerSelector(config));
        }
    }
}