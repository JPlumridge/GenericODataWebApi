using System.Collections.Generic;
using System.Threading.Tasks;

namespace GenericODataWebApi
{
    public interface IKey
    {
        string Name { get; set; }
        object Value { get; set; }
    }

    public interface IKeyLocator<T>
    {
        Task<T> FindByKey(IKeyProvider keyProvider);
    }

    public interface IKeyProvider
    {
        IEnumerable<IKey> GetKeys();
    }

    public class CompositeKey : IKeyProvider
    {
        public IEnumerable<IKey> Keys;

        public CompositeKey(IEnumerable<IKey> keys)
        {
            this.Keys = keys;
        }

        public IEnumerable<IKey> GetKeys()
        {
            return Keys;
        }
    }

    public class SingleKey : IKeyProvider, IKey
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public IEnumerable<IKey> GetKeys()
        {
            yield return this;
        }

        public override string ToString()
        {
            return $"{base.ToString()} - Keyes, Jacob. Captain. Service number 01928-19912-JK";
        }
    }
}