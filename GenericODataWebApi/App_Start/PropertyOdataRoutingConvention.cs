using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.OData.Routing;
using System.Web.OData.Routing.Conventions;

namespace GenericODataWebApi
{
    public class PropertyODataRoutingConvention : NavigationSourceRoutingConvention
    {
        private const string ActionName = "GetProperty";

        public override string SelectAction(ODataPath odataPath, HttpControllerContext controllerContext, ILookup<string, HttpActionDescriptor> actionMap)
        {
            if ((controllerContext.Request.Method == HttpMethod.Get) &&
                (odataPath.PathTemplate.StartsWith("~/entityset/key/property") || odataPath.PathTemplate.StartsWith("~/entityset/key/navigation")) &&
                actionMap.Contains(ActionName))
            {
                var keySegment = odataPath.Segments[1] as KeyValuePathSegment;
                var propSegment = odataPath.Segments[2] as PropertyAccessPathSegment;
                var navSegment = odataPath.Segments[2] as NavigationPathSegment;
                
                var propName = propSegment == null ? navSegment.NavigationPropertyName : propSegment.PropertyName; 
                
                controllerContext.RouteData.Values[ODataRouteConstants.Key] = keySegment.Value;
                controllerContext.RouteData.Values["propertyName"] = propName;

                if (actionMap.Contains(ActionName))
                    return ActionName;
            }
            return null;
        }
    }
}