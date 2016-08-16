using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.OData;

namespace GenericODataWebApi.DataProvider
{
    public static class QueryableExtensions
    {
        public static T FindByKey<T>(this IQueryable<T> sourceQueryable, Dictionary<string, object> keyValues)
        {
            var entityType = typeof(T);
            var keyProps = keyValues.Select(kvp => new { value = kvp.Value, prop = entityType.GetProperty(kvp.Key) });
            var match = sourceQueryable.SingleOrDefault(i => keyProps.All(kp => kp.prop.GetValue(i) == kp.value));

            return match;
        }
    }

    public class QueryableDataProvider<TEntity> : IODataProvider<TEntity> where TEntity : class
    {
        private IQueryable<TEntity> SourceQueryable { get; }
        //public IKeyResolutionStrategy KeyResolver { get; set; }

        public QueryableDataProvider(IQueryable<TEntity> sourceQueryable/*, IKeyResolutionStrategy keyResolver*/)
        {
            this.SourceQueryable = sourceQueryable;
            //this.KeyResolver = keyResolver;
        }
        public Task Add(TEntity item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int key)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> Get()
        {
            return SourceQueryable;
        }

        public Task<TEntity> GetByKey(Dictionary<string, object> keyValues)
        {
            return Task.FromResult(SourceQueryable.FindByKey(keyValues));
        }

        public Task<IQueryable<TEntity>> GetByKeyAsQueryable(Dictionary<string, object> keyValues)
        {
            throw new NotImplementedException();
        }

        public bool KeyMatchesEntity(int key, TEntity item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Replace(int key, TEntity item)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> Update(int key, Delta<TEntity> deltaEntity)
        {
            throw new NotImplementedException();
        }
    }
}
