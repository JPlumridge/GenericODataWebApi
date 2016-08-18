using System.Collections.Generic;

namespace GenericODataWebApi
{
    public class CompositeKey : IKeyProvider
    {
        public IEnumerable<ISingleKey> Keys { get; set; }

        public CompositeKey(IEnumerable<ISingleKey> keys)
        {
            this.Keys = keys;
        }

        public IEnumerable<ISingleKey> GetKeys()
        {
            return Keys;
        }
    }
}