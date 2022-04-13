using System.Threading.Tasks;

namespace Orchesflow.Orchestration
{
    public interface IFallbackable
    {
        Task Fallback();
    }
}