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
        Task<SingleResult<TEdmEntity>> Get(IKeyProvider keyProvider);
        Task<IHttpActionResult> GetProperty(IKeyProvider keyProvider, string propertyName);
        Task<IHttpActionResult> Post(TEdmEntity item);
        Task<IHttpActionResult> Put([FromODataUri] IKeyProvider keyProvider, TEdmEntity update);
        Task<IHttpActionResult> Delete([FromODataUri] IKeyProvider keyProvider);
    }
}