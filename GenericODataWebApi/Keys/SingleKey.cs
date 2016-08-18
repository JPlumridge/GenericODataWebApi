using System.Collections.Generic;

namespace GenericODataWebApi
{
    public class SingleKey : IKeyProvider, ISingleKey
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public IEnumerable<ISingleKey> GetKeys()
        {
            yield return this;
        }

        public override string ToString()
        {
            return $"{base.ToString()} - Keyes, Jacob. Captain. Service number 01928-19912-JK";
        }
    }
}