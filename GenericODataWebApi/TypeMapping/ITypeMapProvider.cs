using System;
using System.Linq;

namespace GenericODataWebApi
{
    public interface ITypeMapProvider
    {
        IQueryable<TDestination> ProjectTo<TDestination>(IQueryable source);
        TDestination Map<TDestination>(object source);
        GenericODataWebApi.TypeMap ResolveTypeMapFromSource(Type entitySourceType);
    }
}