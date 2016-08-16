using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericODataWebApi.DataProvider
{
    public interface IKeyResolutionStrategy
    {
        Task<object> ResolveKey<TEntity>();
    }
}
