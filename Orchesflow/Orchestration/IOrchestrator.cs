using MediatR;
using Orchesflow.Models;
using System.Threading.Tasks;

namespace Orchesflow.Orchestration
{
    public interface IOrchestrator
    {
        Task<RequestResult> SendCommand<T>(IRequest<T> request);
        Task<RequestResult> SendQuery<T>(IRequest<T> request);
    }
}
