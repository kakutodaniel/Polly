using System.Threading.Tasks;

namespace HelloPolly.API
{
    public interface IService
    {
        Task<string> Get();

    }
}
