using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.OData;
using GenericODataWebApi;
using GenericODataWebApi.DataProvider;

namespace GenericODataWebApi.EntityFramework
{
    public class EntityFrameworkODataProvider<TEntity> : IODataProvider<TEntity> where TEntity : class
    {
        private DbContext db;
        private IKeyLocatorStrategy<TEntity> KeyLocatorStrategy { get; }

        public EntityFrameworkODataProvider(DbContext dbContext, IKeyLocatorStrategy<TEntity> keyLocator)
        {
            db = dbContext;
            this.KeyLocatorStrategy = keyLocator;
        }

        public IQueryable<TEntity> Get()
        {
            return db.Set<TEntity>().AsNoTracking();
        }

        public async Task<IQueryable<TEntity>> GetByKeyAsQueryable(IKeyProvider keyProvider)
        {
            var wrapper = new[] { await GetByKey(keyProvider) };
            return wrapper.AsQueryable();
        }

        public async Task<TEntity> GetByKey(IKeyProvider keyProvider)
        {
            return await KeyLocatorStrategy.FindByKey(keyProvider);
        }

        public async Task<bool> Delete(IKeyProvider keyProvider)
        {
            var toDelete = await GetByKey(keyProvider);
            if (toDelete == null)
                return false;

            db.Set<TEntity>().Remove(toDelete);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task Add(TEntity item)
        {
            db.Set<TEntity>().Add(item);
            await db.SaveChangesAsync();
        }

        public async Task<bool> Replace(TEntity item)
        {
            db.Entry(item).State = EntityState.Modified;
            try
            {
                await db.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
                //if (!ProductExists(key))
                //{
                //    return NotFound();
                //}
            }
        }

        public async Task<TEntity> Update(IKeyProvider keyProvider, Delta<TEntity> deltaEntity)
        {
            var matching = await GetByKey(keyProvider);

            if (matching == null)
                return null;

            deltaEntity.Patch(matching);

            try
            {
                await db.SaveChangesAsync();
                return matching;
            }
            catch (DbUpdateConcurrencyException)
            {
                //if (!ProductExists(key))
                //{
                //    return NotFound();
                //}
                //else
                //{
                throw;
                //}
            }
        }

        public async Task<bool> KeyMatchesEntity(IKeyProvider keyProvider, TEntity item)
        {
            var efAwareItem = db.Set<TEntity>().Attach(item);

            var keyItem = await GetByKey(keyProvider);
            return efAwareItem == keyItem;
        }
    }
}