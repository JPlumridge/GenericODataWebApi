using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericODataWebApi.DataProvider;
using Microsoft.Practices.Unity;

namespace GenericODataWebApi.Extensions
{
    public static class UnityExtensions
    {
        /// <summary>
        /// Uses the given IQueryable to register a basic QueryableDataProvider for that entity
        /// </summary>
        /// <typeparam name="TEntity">Type of entity to register queryable for</typeparam>
        /// <param name="container"></param>
        /// <param name="sourceQueryable">Queryable which provides access to Entity, and which will be registered</param>
        public static void RegisterQueryableODataSource<TEntity>(this IUnityContainer container, IQueryable<TEntity> sourceQueryable) where TEntity : class
        {
            container.RegisterQueryableKeyLocationStrategy(sourceQueryable);
            container.RegisterType<IODataProvider<TEntity>, QueryableDataProvider<TEntity>>(new InjectionConstructor(
                                                                                                sourceQueryable, 
                                                                                                new ResolvedParameter<IKeyLocatorStrategy<TEntity>>()));
        }

        /// <summary>
        /// Registers the default key locator for the given IQueryable
        /// </summary>
        /// <typeparam name="TEntity">The type for which to use the default key locator</typeparam>
        /// <param name="container"></param>
        /// <param name="sourceQueryable">Queryable data source for which to use the default key locator</param>
        public static void RegisterQueryableKeyLocationStrategy<TEntity>(this IUnityContainer container, IQueryable<TEntity> sourceQueryable)
        {
            container.RegisterType<IKeyLocatorStrategy<TEntity>, QueryableKeyLocatorStrategy<TEntity>>(new InjectionConstructor(sourceQueryable));
        }
    }
}
