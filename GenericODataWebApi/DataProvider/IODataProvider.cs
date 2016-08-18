using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.OData;
using GenericODataWebApi;

namespace GenericODataWebApi
{
    public interface IODataProvider<TEntity> where TEntity : class
    {
        Task Add(TEntity item);
        Task<bool> Delete(IKeyProvider keyProvider);
        IQueryable<TEntity> Get();
        Task<TEntity> GetByKey(IKeyProvider keyProvider);
        Task<IQueryable<TEntity>> GetByKeyAsQueryable(IKeyProvider keyProvider);
        Task<bool> KeyMatchesEntity(IKeyProvider keyProvider, TEntity item);
        Task<bool> Replace(TEntity item);
        Task<TEntity> Update(IKeyProvider keyProvider, Delta<TEntity> deltaEntity);
    }
}