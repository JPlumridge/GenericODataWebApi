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
    public class EntityFrameworkPrimaryKeyLocatorStrategy<TEntity> : IKeyLocatorStrategy<TEntity> where TEntity : class
    {
        private DbContext db;

        public EntityFrameworkPrimaryKeyLocatorStrategy(DbContext dbContext)
        {
            db = dbContext;
        }

        public async Task<TEntity> FindByKey(IKeyProvider keyProvider)
        {
            var keys = keyProvider.GetKeys().Select(k => k.Value);
            return await db.Set<TEntity>().FindAsync(keys.ToArray());
        }
    }


    public class EntityFrameworkODataProvider<TEntity> : IODataProvider<TEntity> where TEntity : class
    {
        private DbContext db;
        private IKeyLocatorStrategy<TEntity> KeyLocatorStrategy { get; }

        public EntityFrameworkODataProvider(DbContext dbContext)
        {
            db = dbContext;
            //this.KeyLocator = new EntityFrameworkPrimaryKeyLocator<TEntity>(dbContext); //todo: decouple
            this.KeyLocatorStrategy = new QueryableKeyLocatorStrategy<TEntity>(dbContext.Set<TEntity>());
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

        //// <summary>
        //// If your key is NOT the primary key, override this!
        //// </summary>
        //public virtual async Task<TEntity> GetByKey(int key)
        //{
        //    //todo: ascertain what the key actually is
        //    return await db.Set<TEntity>().FindAsync(key);
        //}

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