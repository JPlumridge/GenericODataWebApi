using System;

namespace GenericODataWebApi
{
    public class TypeMap
    {
        public TypeMap(Type source, Type destination )
        {
            SourceType = source;
            DestinationType = destination;
        }
        public Type SourceType { get; set; }
        public Type DestinationType { get; set; }
    }
}