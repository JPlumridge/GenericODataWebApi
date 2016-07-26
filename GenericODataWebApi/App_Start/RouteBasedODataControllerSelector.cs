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

        private HttpControllerDescriptor GetDescriptorFromEntitySet(HttpRequestMessage request, string entitySetName)
        {
            var httpConfig = request.GetConfiguration();

            var odataRoute = httpConfig.Routes.First(r => r is ODataRoute) as ODataRoute;
            var edmModel = odataRoute.PathRouteConstraint.EdmModel;

            var set = edmModel.EntityContainer.FindEntitySet(entitySetName);
            
            if (set != null)
            {
                var type = set.Type as EdmCollectionType;
                var realType = edmModel.GetAnnotationValue<ClrTypeAnnotation>(type.ElementType.Definition).ClrType;


                if (GenericODataConfig.TypeMapping.CustomMappingEnabled)
                {
                    var typeMap = GenericODataConfig.TypeMapping.ResolveTypeMap(realType);

                    var controllerType = typeof(TypeMappedEntityFrameworkODataController<,>);
                    controllerType = controllerType.MakeGenericType(typeMap.SourceType, typeMap.DestinationType);

                    return new HttpControllerDescriptor(GlobalConfiguration.Configuration, "TypeMappedEntityFrameworkOData", controllerType);
                }
                else
                {
                    var controllerType = typeof (SimpleEntityFrameworkODataController<>);
                    controllerType = controllerType.MakeGenericType(realType);

                    return new HttpControllerDescriptor(GlobalConfiguration.Configuration, "EntityFrameworkOData", controllerType);
                }
            }
            return null;
        }
    }
}