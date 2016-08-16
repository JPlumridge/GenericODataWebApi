using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.OData;

namespace GenericODataWebApi
{
    public interface IODataProvider<TEntity> where TEntity : class
    {
        Task Add(TEntity item);
        Task<bool> Delete(int key);
        IQueryable<TEntity> Get();
        Task<TEntity> GetByKey(Dictionary<string, object> keyValues);
        Task<IQueryable<TEntity>> GetByKeyAsQueryable(Dictionary<string, object> keyValues);
        bool KeyMatchesEntity(int key, TEntity item);
        Task<bool> Replace(int key, TEntity item);
        Task<TEntity> Update(int key, Delta<TEntity> deltaEntity);
    }
}