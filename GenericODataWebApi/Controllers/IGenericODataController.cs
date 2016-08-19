using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace GenericODataWebApi
{
    interface IGenericODataController<TEdmEntity>
    {
        IQueryable<TEdmEntity> Get();
        Task<SingleResult<TEdmEntity>> GetByKey(IKeyProvider keyProvider);
        Task<IHttpActionResult> GetProperty(IKeyProvider keyProvider, string propertyName);
        Task<IHttpActionResult> Post(TEdmEntity item);
        Task<IHttpActionResult> Put(IKeyProvider keyProvider, TEdmEntity update);
        Task<IHttpActionResult> Delete(IKeyProvider keyProvider);
    }
}