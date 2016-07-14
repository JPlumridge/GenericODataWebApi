using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web.Http.Controllers;
using System.Web.OData;
using System.Web.OData.Query;

namespace GenericODataWebApi
{
    public class EnableQueryCustomValidationAttribute : EnableQueryAttribute
    {
        private void ApplyValidationSettings(Type edmType)
        {
            ODataValidationSettings validationSettings = GenericODataConfig.SettingsFor(edmType) ?? GenericODataConfig.GlobalSettings;
            if (validationSettings == null)
                return;

            AllowedArithmeticOperators = validationSettings.AllowedArithmeticOperators;
            AllowedFunctions = validationSettings.AllowedFunctions;
            AllowedLogicalOperators = validationSettings.AllowedLogicalOperators;
            AllowedQueryOptions = validationSettings.AllowedQueryOptions;
            MaxAnyAllExpressionDepth = validationSettings.MaxAnyAllExpressionDepth;
            MaxExpansionDepth = validationSettings.MaxExpansionDepth;
            MaxNodeCount = validationSettings.MaxNodeCount;
            MaxOrderByNodeCount = validationSettings.MaxOrderByNodeCount;

            if (validationSettings.AllowedOrderByProperties.Any())
                AllowedOrderByProperties = validationSettings.AllowedOrderByProperties.Aggregate((s, p) => s += $",{p}");

            if (validationSettings.MaxSkip.HasValue)
                MaxSkip = validationSettings.MaxSkip.Value;
            if (validationSettings.MaxTop.HasValue)
                MaxTop = validationSettings.MaxTop.Value;
        }

        public override void ValidateQuery(HttpRequestMessage request, ODataQueryOptions queryOptions)
        {
            var controllerType = request.Properties.Values.OfType<HttpActionDescriptor>().SingleOrDefault()?.ControllerDescriptor?.ControllerType;
            var genericODataInterface = controllerType?.GetInterfaces().SingleOrDefault(i => i.IsGenericType && (i.GetGenericTypeDefinition() == typeof (IGenericODataController<>)));

            if (genericODataInterface != null)
            {
                var typeOfEdmEntity = genericODataInterface.GetGenericArguments()[0];
                ApplyValidationSettings(typeOfEdmEntity);
            }

            base.ValidateQuery(request, queryOptions);
        }
    }
}