using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GenericODataWebApi.Extensions
{
    internal static class TypeExtensions
    {
        public static IEnumerable<MemberInfo> GetMetadataMembers(this Type type)
        {
            var metadataAttr = type.GetCustomAttributes<MetadataTypeAttribute>().FirstOrDefault();

            IEnumerable<MemberInfo> metadataMembers = metadataAttr?.MetadataClassType.GetProperties();
            metadataMembers = metadataMembers?.Concat(metadataAttr.MetadataClassType.GetFields());

            return metadataMembers ?? Enumerable.Empty<MemberInfo>();
        }
    }
}
