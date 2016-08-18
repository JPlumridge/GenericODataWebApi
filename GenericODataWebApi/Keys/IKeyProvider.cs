using System.Collections.Generic;

namespace GenericODataWebApi
{
    public interface IKeyProvider
    {
        IEnumerable<ISingleKey> GetKeys();
    }
}