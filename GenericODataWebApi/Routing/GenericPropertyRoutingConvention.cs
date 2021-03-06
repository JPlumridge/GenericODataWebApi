using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.OData;
using System.Web.OData.Routing;
using System.Web.OData.Routing.Conventions;
using Microsoft.OData.Edm;

namespace GenericODataWebApi
{
    public class GenericPropertyRoutingConvention : NavigationSourceRoutingConvention //todo: user NavigationRoutingConvention? And "replace" existing one with this, and return base.SelectAction
    {
        //todo: don't rely on controllers/interface having matching name, but specify in a single place somehow.
        private const string ActionName = "GetProperty";
        private const string ParameterName = "propertyName";

        public override string SelectAction(ODataPath odataPath, HttpControllerContext controllerContext, ILookup<string, HttpActionDescriptor> actionMap)
        {
            // Need to support bloody stuff like this: ~/entityset/key/navigation/navigation/key

            if ((controllerContext.Request.Method == HttpMethod.Get) &&
                (odataPath.PathTemplate.StartsWith("~/entityset/key/property") || odataPath.PathTemplate.StartsWith("~/entityset/key/navigation")) &&
                actionMap.Contains(ActionName))
            {
                var propSegment = odataPath.Segments[2] as PropertyAccessPathSegment;
                var navSegment = odataPath.Segments[2] as NavigationPathSegment;

                var httpConfig = controllerContext.Request.GetConfiguration();
                var odataRoute = httpConfig.Routes.First(r => r is ODataRoute) as ODataRoute;
                var edmModel = odataRoute.PathRouteConstraint.EdmModel;

                IEdmElement element = propSegment?.Property ?? navSegment.NavigationProperty;
                var propInfoAnnotation = edmModel.GetAnnotationValue<ClrPropertyInfoAnnotation>(element);

                var propName = propInfoAnnotation?.ClrPropertyInfo?.Name;
                propName = propName ?? (propSegment == null ? navSegment.NavigationPropertyName : propSegment.PropertyName);

                controllerContext.RouteData.Values[ParameterName] = propName;
                return ActionName;
            }
            return null;
        }
    }

    //todo: This probably needs to be an abstract base class, instead of having to be added as the first routing convention
    public class KeyDetectorRoutingConvention : EntityRoutingConvention
    {
        private const string ParameterName = "keyProvider";

        public override string SelectAction(ODataPath odataPath, HttpControllerContext controllerContext, ILookup<string, HttpActionDescriptor> actionMap)
        {
            if (odataPath.PathTemplate.StartsWith("~/entityset/key"))
            {
                var keySegment = odataPath.Segments[1] as KeyValuePathSegment;
                var keys = keySegment.Segment.Keys.Select(k => new SingleKey { Name = k.Key, Value = k.Value });

                //Should be impossible to get this far with no key
                var provider = keys.Count() > 1 ? (IKeyProvider)new CompositeKey(keys) : (IKeyProvider)keys.Single();

                controllerContext.RouteData.Values[ParameterName] = provider;
            }
            return null;
        }
    }

    public class KeyRoutingConvention : EntityRoutingConvention
    {
        private const string ActionName = "GetByKey";

        public override string SelectAction(ODataPath odataPath, HttpControllerContext controllerContext, ILookup<string, HttpActionDescriptor> actionMap)
        {
            if (odataPath.PathTemplate == "~/entityset/key" && (controllerContext.Request.Method == HttpMethod.Get) && (actionMap.Contains(ActionName)))
            {
                return ActionName;
            }
            return null;
        }
    }
}