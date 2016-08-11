using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;

namespace GenericODataWebApi
{
    public class TypeMappedEntityFrameworkODataController<TEntity, TModel> : EntityFrameworkODataController<TEntity>/*, IGenericODataController<TModel>*/ where TEntity : class// where TModel : class //: GenericODataController<TEntity> where TEntity : class
    {
        public TypeMappedEntityFrameworkODataController(DbContext dbContext) : base(dbContext)
        {
        }

        //todo: have abstract base class implement base get/delete e.t.c, then have sub class do the actual work.

        //need to use "new" as odata WebApi is convention based - method names mean something
        [EnableQueryCustomValidation]
        [IfODataMethodEnabled(ODataOperations.Get)]
        public IQueryable<TModel> Get()
        {
            return DataProvider.Get().ProjectUsingCustom<TModel>();
        }

        [EnableQueryCustomValidation]
        [IfODataMethodEnabled(ODataOperations.Get)]
        public SingleResult<TModel> Get([FromODataUri] int key)
        {
            var result = DataProvider.GetByKeyAsQueryable(key).Result.ProjectUsingCustom<TModel>();
            return SingleResult.Create<TModel>(result);
        }

        //todo: less duplication with base
        [IfODataMethodEnabled(ODataOperations.Get)]
        public IHttpActionResult GetProperty(int key, string propertyName)
        {
            var prop = GetPropertyInfo<TModel>(propertyName);
            var container = DataProvider.GetByKeyAsQueryable(key).Result.ProjectUsingCustom<TModel>().First();

            dynamic value = prop.GetValue(container);
            return Ok(value);
        }

        protected PropertyInfo GetPropertyInfo<TParent>(string propertyName)
        {
            var prop = typeof(TParent).GetProperty(propertyName);
            return prop;
        }

        [IfODataMethodEnabled(ODataOperations.Add)]
        public async Task<IHttpActionResult> Post(TModel item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var converted = item.Map<TEntity>();

            await DataProvider.Add(converted);
            return Created(item);
        }

        [IfODataMethodEnabled(ODataOperations.Update)]
        public async Task<IHttpActionResult> Put([FromODataUri] int key, TModel update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var converted = update.Map<TEntity>();

            if (!DataProvider.KeyMatchesEntity(key, converted))
            {
                return BadRequest();
            }

            if (await DataProvider.Replace(key, converted))
                return Updated(update);
            return NotFound();
        }
    }
}