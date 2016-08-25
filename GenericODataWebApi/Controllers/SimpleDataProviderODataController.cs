using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.OData;
using System.Web.OData.Batch;
using System.Web.OData.Extensions;
using System.Web.OData.Routing;

namespace GenericODataWebApi
{
    public class SimpleDataProviderODataController<TEntity> : DataProviderODataController<TEntity>, IGenericODataController<TEntity> where TEntity : class
    {
        public SimpleDataProviderODataController(IODataProvider<TEntity> dataProvider) : base( dataProvider)
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
        public async Task<SingleResult<TEntity>> GetByKey([FromRouteData]IKeyProvider keyProvider)
        {
            var result = await DataProvider.GetByKeyAsQueryable(keyProvider);
            return SingleResult.Create<TEntity>(result);
        }

        [EnableQueryCustomValidation]
        [IfODataMethodEnabled(ODataOperations.Get)]
        public async Task<IHttpActionResult> GetProperty([FromRouteData]IKeyProvider keyProvider, string propertyName)
        {
            var prop = GetPropertyInfo<TEntity>(propertyName);
            var container = await DataProvider.GetByKey(keyProvider);
            
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
        public async Task<IHttpActionResult> Put([FromRouteData] IKeyProvider keyProvider, TEntity update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await DataProvider.KeyMatchesEntity(keyProvider, update))
            {
                return BadRequest();
            }

            if (await DataProvider.Replace(update))
                return Updated(update);
            return NotFound();
        }

        [IfODataMethodEnabled(ODataOperations.Update)]
        public async Task<IHttpActionResult> Patch([FromRouteData] IKeyProvider keyProvider, Delta<TEntity> deltaEntity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updated = await DataProvider.Update(keyProvider, deltaEntity);

            if (updated != null)
                return Updated(updated);
            return NotFound();
        }
    }
}
