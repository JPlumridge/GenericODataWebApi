using System;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace GenericODataWebApi
{
    public class SimpleDataProviderODataController<TEntity> : DataProviderODataController<TEntity>, IGenericODataController<TEntity> where TEntity : class
    {
        public SimpleDataProviderODataController(DbContext dbContext) : base(dbContext)
        {
        }

        [EnableQueryCustomValidation]
        [IfODataMethodEnabled(ODataOperations.Get)]
        public IQueryable<TEntity> Get()
        {
            return DataProvider.Get();
        }

        [EnableQueryCustomValidation]
        [IfODataMethodEnabled(ODataOperations.Get)]
        public async Task<SingleResult<TEntity>> Get([FromODataUri] int key)
        {
            var result = await DataProvider.GetByKeyAsQueryable(key);
            return SingleResult.Create<TEntity>(result);
        }

        [IfODataMethodEnabled(ODataOperations.Get)]
        public async Task<IHttpActionResult> GetProperty(int key, string propertyName)
        {
            var prop = GetPropertyInfo<TEntity>(propertyName);
            var container = await DataProvider.GetByKey(key);
            
            dynamic value = prop.GetValue(container);
            return Ok(value);
        }

        protected PropertyInfo GetPropertyInfo<TParent>(string propertyName)
        {
            var prop = typeof (TParent).GetProperty(propertyName);
            return prop;
        }

        [IfODataMethodEnabled(ODataOperations.Add)]
        public async Task<IHttpActionResult> Post(TEntity item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await DataProvider.Add(item);
            return Created(item);
        }

        [IfODataMethodEnabled(ODataOperations.Update)]
        public async Task<IHttpActionResult> Put([FromODataUri] int key, TEntity update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!DataProvider.KeyMatchesEntity(key, update))
            {
                return BadRequest();
            }

            if (await DataProvider.Replace(key, update))
                return Updated(update);
            return NotFound();
        }

        [IfODataMethodEnabled(ODataOperations.Update)]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<TEntity> deltaEntity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updated = await DataProvider.Update(key, deltaEntity);

            if (updated != null)
                return Updated(updated);
            return NotFound();
        }
    }
}
