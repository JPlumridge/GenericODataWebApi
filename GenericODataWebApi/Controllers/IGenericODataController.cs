using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace GenericODataWebApi
{
    interface IGenericODataController<TEdmEntity>
    {
        IQueryable<TEdmEntity> Get();
        SingleResult<TEdmEntity> Get(int key);
        IHttpActionResult GetProperty(int key, string propertyName);
        Task<IHttpActionResult> Post(TEdmEntity item);
        Task<IHttpActionResult> Put([FromODataUri] int key, TEdmEntity update);
        Task<IHttpActionResult> Delete([FromODataUri] int key);
    }
}