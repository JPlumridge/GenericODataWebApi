using System;
using System.Linq;

namespace GenericODataWebApi
{
    public class CustomTypeMapping
    {
        private ITypeMapProvider TypeMapProvider { get; set; }
        internal bool CustomMappingEnabled => TypeMapProvider != null;

        public void EnableCustomTypeMapping(ITypeMapProvider provider)
        {
            TypeMapProvider = provider;
        }

        internal TypeMap ResolveTypeMap(Type sourceType)
        {
            return TypeMapProvider.ResolveTypeMapFromSource(sourceType);
        }

        internal TDestination Map<TDestination>(object source)
        {
            return TypeMapProvider.Map<TDestination>(source);
        }

        internal IQueryable<TDestination> ProjectUsingCustom<TDestination>(IQueryable source)
        {
            return TypeMapProvider.ProjectTo<TDestination>(source);
        }
    }
}