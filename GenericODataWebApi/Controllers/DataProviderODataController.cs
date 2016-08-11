﻿using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace GenericODataWebApi
{
    //todo: rename this
    public abstract class DataProviderODataController<TEntity> : ODataController where TEntity : class
    {
        protected IODataProvider<TEntity> DataProvider { get; }
        protected DataProviderODataController(DbContext dbContext)
        {
            //todo: decouple this, provide an interface through which any data can be accessed, EF or other
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