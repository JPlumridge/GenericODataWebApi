using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace GenericODataWebApi
{
    //todo: rename this
    public abstract class EntityFrameworkODataControllerBASE<TEntity> : ODataController where TEntity : class
    {
        protected EntityFrameworkODataProvider<TEntity> DataProvider { get; }
        protected EntityFrameworkODataControllerBASE(DbContext dbContext)
        {
            DataProvider = new EntityFrameworkODataProvider<TEntity>(dbContext);
        }

        [IfODataMethodEnabled(ODataOperations.Delete)]
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            if (await DataProvider.Delete(key))
                return StatusCode(HttpStatusCode.NoContent);

            return NotFound();
        }
    }
}