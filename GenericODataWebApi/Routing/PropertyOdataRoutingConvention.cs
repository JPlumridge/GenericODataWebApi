using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;
using System.Web.Http.ValueProviders.Providers;
using System.Web.OData.Routing;
using System.Web.OData.Routing.Conventions;
using System.Xml.Serialization;
using Microsoft.OData.Edm;

namespace GenericODataWebApi
{
    public class PropertyODataRoutingConvention : NavigationSourceRoutingConvention //todo: user NavigationRoutingConvention! And "replace" existing one with this, and return base.SelectAction
    {
        private const string ActionName = "GetProperty";
        private const string ParameterName = "propertyName";

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
                
                //controllerContext.RouteData.Values[ODataRouteConstants.Key] = keySegment.Value;
                controllerContext.RouteData.Values[ParameterName] = propName;

                if (actionMap.Contains(ActionName))
                    return ActionName;
            }

            return null;
        }
    }

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
                IKeyProvider provider = keys.Count() > 1 ? (IKeyProvider)new CompositeKey(keys) : (IKeyProvider)keys.Single();

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
            if (odataPath.PathTemplate == "~/entityset/key" && (controllerContext.Request.Method == HttpMethod.Get))
            {
                if (actionMap.Contains(ActionName))
                    return ActionName;
            }

            return null;
        }
    }

    public class FromRouteDataAttribute : ModelBinderAttribute
    {
        public override IEnumerable<ValueProviderFactory> GetValueProviderFactories(HttpConfiguration configuration)
        {
            yield return new RouteDataObjectValueProviderFactory();

            //ValueProviderFactory factory = new RouteDataValueProviderFactory();

            //var defaults = base.GetValueProviderFactories(configuration);

            //return defaults.Concat(new[] {factory});
        }
    }

    public class RouteDataObjectValueProviderFactory : ValueProviderFactory
    {
        public override IValueProvider GetValueProvider(HttpActionContext actionContext)
        {
            return new RouteDataObjectValueProvider(actionContext);
        }
    }

    public class RouteDataObjectValueProvider : IValueProvider
    {
        private HttpActionContext ActionContext { get; }

        public RouteDataObjectValueProvider(HttpActionContext actionContext)
        {
            ActionContext = actionContext;
        }
        public bool ContainsPrefix(string prefix)
        {
            return ActionContext.ControllerContext.RouteData.Values.ContainsKey(prefix);
        }

        public ValueProviderResult GetValue(string key)
        {
            //todo: handle possible exception from Single?
            var targetParameter = ActionContext.ActionDescriptor.GetParameters().Single(p => p.ParameterName == key);
            var routeData = ActionContext.ControllerContext.RouteData;

            var candidateValue = routeData.Values[key];
            var isCorrectType = targetParameter.ParameterType.IsInstanceOfType(candidateValue);

            return new ValueProviderResult(isCorrectType ? candidateValue : null, key, CultureInfo.InvariantCulture);
        }
    }
}