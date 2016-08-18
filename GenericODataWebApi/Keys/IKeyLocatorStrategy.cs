using System.Threading.Tasks;

namespace GenericODataWebApi
{
    public interface IKeyLocatorStrategy<T>
    {
        Task<T> FindByKey(IKeyProvider keyProvider);
    }
}