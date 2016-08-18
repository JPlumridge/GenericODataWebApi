using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
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
        }
        public Task Add(TEntity item)
        {
            throw GetMethodNotAllowedException();
        }

        public Task<bool> Delete(IKeyProvider keyProvider)
        {
            throw GetMethodNotAllowedException();
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
            throw GetMethodNotAllowedException();
        }

        public Task<bool> Replace(TEntity item)
        {
            throw GetMethodNotAllowedException();
        }

        public Task<TEntity> Update(IKeyProvider keyProvider, Delta<TEntity> deltaEntity)
        {
            throw GetMethodNotAllowedException();
        }

        private HttpResponseException GetMethodNotAllowedException()
        {
            var message = new HttpResponseMessage(HttpStatusCode.MethodNotAllowed) { Content = new StringContent("The data source does not allow this operation") };
            return new HttpResponseException(message);
        }
    }
}
