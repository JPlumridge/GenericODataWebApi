using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.OData;
using System.Web.OData.Builder;
using System.Web.OData.Routing;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;

namespace GenericODataWebApi
{internal class RouteBasedODataControllerSelector : DefaultHttpControllerSelector
    {
        public RouteBasedODataControllerSelector(HttpConfiguration config) : base(config)
        {
        }

        //todo: make this better
        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            try
            {
                var desc = base.SelectController(request);
                return desc;
            }
            catch //If this happens, and the below code can't find an entity set, the exception will be thrown again
            { }
            
            var routeName = request.GetRouteData().Values["controller"] as string;

            //Call this now to either let exception propogate up, or select correct controller
            return GetDescriptorFromEntitySet(request, routeName) ?? base.SelectController(request); 
        }

        private static Dictionary<string, Type> TypeCache = new Dictionary<string, Type>();

        private Type GetClrTypeFromEdmType(string typeName)
        {
            //Bit of a shame that OData doesn't "remember" what clr types are mapped to EDM types
            if (!TypeCache.ContainsKey(typeName))
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.GlobalAssemblyCache);
                var type = assemblies.First(a => a.GetType(typeName) != null).GetType(typeName);
                try
                {
                    TypeCache.Add(typeName, type);
                }
                catch (ArgumentException)
                {
                    //This just means that two threads were able to attempt to add
                }
            }

            return TypeCache[typeName];
        }

        private HttpControllerDescriptor GetDescriptorFromEntitySet(HttpRequestMessage request, string entitySetName)
        {
            var httpConfig = request.GetConfiguration();

            var odataRoute = httpConfig.Routes.First(r => r is ODataRoute) as ODataRoute;
            var set = odataRoute.PathRouteConstraint.EdmModel.EntityContainer.FindEntitySet(entitySetName);
            
            if (set != null)
            {
                var type = set.Type as EdmCollectionType;
                var typeName = type.ElementType.FullName();

                var realType = GetClrTypeFromEdmType(typeName);

                if (GenericODataConfig.TypeMapping.CustomMappingEnabled)
                {
                    var typeMap = GenericODataConfig.TypeMapping.ResolveTypeMap(realType);

                    var controllerType = typeof(TypeMappedEntityFrameworkODataController<,>);
                    controllerType = controllerType.MakeGenericType(typeMap.SourceType, typeMap.DestinationType);

                    return new HttpControllerDescriptor(GlobalConfiguration.Configuration, "TypeMappedEntityFrameworkOData", controllerType);
                }
                else
                {
                    var controllerType = typeof (EntityFrameworkODataController<>);
                    controllerType = controllerType.MakeGenericType(realType);

                    return new HttpControllerDescriptor(GlobalConfiguration.Configuration, "EntityFrameworkOData", controllerType);
                }
            }
            return null;
        }
    }
}