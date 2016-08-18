using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

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
}