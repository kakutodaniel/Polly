using System.Threading;
using System.Threading.Tasks;

namespace HelloPolly.API
{
    public interface IRepository
    {
        Task<string> GetData(CancellationToken ct = default);
    }
}
