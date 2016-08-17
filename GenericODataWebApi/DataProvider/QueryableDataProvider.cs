using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.OData;

namespace GenericODataWebApi.DataProvider
{
    //public static class QueryableExtensions
    //{
    //    public static T FindByKey<T>(this IQueryable<T> sourceQueryable, Dictionary<string, object> keyValues)
    //    {
    //        var entityType = typeof(T);
    //        var keyProps = keyValues.Select(kvp => new { value = kvp.Value, prop = entityType.GetProperty(kvp.Key) });
    //        var match = sourceQueryable.SingleOrDefault(i => keyProps.All(kp => kp.prop.GetValue(i) == kp.value));

    //        return match;
    //    }
    //}

    public class QueryableKeyLocator<T> : IKeyLocator<T>
    {
        private IQueryable<T> SourceQueryable { get; }

        public QueryableKeyLocator(IQueryable<T> sourceQueryable)
        {
            this.SourceQueryable = sourceQueryable;
        }

        public Task<T> FindByKey(IKeyProvider keyProvider)
        {
            var entityType = typeof(T);
            var keys = keyProvider.GetKeys();

            var keyProps = keys.Select(key => new { value = key.Value, prop = entityType.GetProperty(key.Name) });
            var match = SourceQueryable.SingleOrDefault(i => keyProps.All(kp => kp.prop.GetValue(i) == kp.value));

            return Task.FromResult(match);
        }
    }

    public class QueryableDataProvider<TEntity> : IODataProvider<TEntity> where TEntity : class
    {
        private IQueryable<TEntity> SourceQueryable { get; }
        private IKeyLocator<TEntity> KeyLocator { get; } 

        public QueryableDataProvider(IQueryable<TEntity> sourceQueryable)
        {
            this.SourceQueryable = sourceQueryable;
            this.KeyLocator = new QueryableKeyLocator<TEntity>(SourceQueryable); //todo: decouple!
        }
        public Task Add(TEntity item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(IKeyProvider keyProvider)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> Get()
        {
            return SourceQueryable;
        }

        public Task<TEntity> GetByKey(IKeyProvider keyProvider)
        {
            return KeyLocator.FindByKey(keyProvider);
        }

        public Task<IQueryable<TEntity>> GetByKeyAsQueryable(IKeyProvider keyProvider)
        {
            var queryable = new[] {KeyLocator.FindByKey(keyProvider).Result}.AsQueryable();
            return Task.FromResult(queryable);
        }

        public bool KeyMatchesEntity(IKeyProvider keyProvider, TEntity item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Replace(TEntity item)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> Update(IKeyProvider keyProvider, Delta<TEntity> deltaEntity)
        {
            throw new NotImplementedException();
        }
    }
}
