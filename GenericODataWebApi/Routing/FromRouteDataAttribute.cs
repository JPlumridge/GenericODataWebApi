using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;

namespace GenericODataWebApi
{
    public class FromRouteDataAttribute : ModelBinderAttribute
    {
        public override IEnumerable<ValueProviderFactory> GetValueProviderFactories(HttpConfiguration configuration)
        {
            yield return new RouteDataObjectValueProviderFactory();

            //todo: assess allowing default factories to do their thing?
            //var defaults = base.GetValueProviderFactories(configuration);
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
            //todo: figure out if this makes any sense at all. This method is called after GetValue. Why?
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