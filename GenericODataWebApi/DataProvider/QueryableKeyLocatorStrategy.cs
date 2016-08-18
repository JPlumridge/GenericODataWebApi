using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericODataWebApi.DataProvider
{
    public class QueryableKeyLocatorStrategy<T> : IKeyLocatorStrategy<T>
    {
        private IQueryable<T> SourceQueryable { get; }

        public QueryableKeyLocatorStrategy(IQueryable<T> sourceQueryable)
        {
            this.SourceQueryable = sourceQueryable;
        }

        public Task<T> FindByKey(IKeyProvider keyProvider)
        {
            var entityType = typeof(T);
            var keys = keyProvider.GetKeys();

            var keyProps = keys.Select(key => new { value = key.Value, prop = entityType.GetProperty(key.Name) });
            var match = SourceQueryable.SingleOrDefault(i => keyProps.All(kp => (int)kp.prop.GetValue(i) == (int)kp.value));

            return Task.FromResult(match);
        }
    }
}