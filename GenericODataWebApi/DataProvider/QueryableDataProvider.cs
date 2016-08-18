using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.OData;

namespace GenericODataWebApi.DataProvider
{
    public class QueryableDataProvider<TEntity> : IODataProvider<TEntity> where TEntity : class
    {
        private IQueryable<TEntity> SourceQueryable { get; }
        private IKeyLocatorStrategy<TEntity> KeyLocator { get; } 

        public QueryableDataProvider(IQueryable<TEntity> sourceQueryable, IKeyLocatorStrategy<TEntity> keyLocator)
        {
            this.SourceQueryable = sourceQueryable;
            this.KeyLocator = keyLocator;
            //this.KeyLocator = new QueryableKeyLocatorStrategy<TEntity>(SourceQueryable); //todo: decouple!
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

        public Task<bool> KeyMatchesEntity(IKeyProvider keyProvider, TEntity item)
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
