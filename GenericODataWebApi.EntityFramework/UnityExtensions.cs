using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace GenericODataWebApi.EntityFramework
{
    public static class UnityExtensions
    {
        /// <summary>
        /// Uses the given DbContext type to register an EntityFrameworkODataProvider as the applications IODataProvider
        /// </summary>
        /// <typeparam name="TContext">Your DbContext type</typeparam>
        /// <param name="container"></param>
        public static void RegisterEntityFrameworkOData<TContext>(this IUnityContainer container)
        {
            container.RegisterEntityFrameworkOData(typeof(TContext));
        }
        /// <summary>
        /// Uses the given DbContext type to register an EntityFrameworkODataProvider as the applications IODataProvider
        /// </summary>
        /// <param name="container">Your DbContext type</param>
        /// <param name="dbContextType"></param>
        public static void RegisterEntityFrameworkOData(this IUnityContainer container, Type dbContextType)
        {
            container.RegisterType(typeof(DbContext), dbContextType);
            container.RegisterType(typeof(IKeyLocatorStrategy<>), typeof(EntityFrameworkPrimaryKeyLocatorStrategy<>));
            container.RegisterType(typeof(IODataProvider<>), typeof(EntityFrameworkODataProvider<>));
        }
    }
}
