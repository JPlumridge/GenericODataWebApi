using System.Linq;

namespace GenericODataWebApi
{
    public static class MappingExtensions
    {
        public static IQueryable<TDestination> ProjectUsingCustom<TDestination>(this IQueryable source)
        {
            return GenericODataConfig.TypeMapping.ProjectUsingCustom<TDestination>(source);
        }

        public static TDestination Map<TDestination>(this object source)
        {
            return GenericODataConfig.TypeMapping.Map<TDestination>(source);
        }
    }
}