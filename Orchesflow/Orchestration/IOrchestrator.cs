using MediatR;
using Orchesflow.Models;
using System.Threading.Tasks;

namespace Orchesflow.Orchestration
{
    public interface IOrchestrator
    {
        Task<RequestResult> SendCommand<TRequest, TResponse>(IRequest<TResponse> request)
            where TRequest : IRequest<TResponse>;
        Task<RequestResult> SendQuery<T>(IRequest<T> request);
    }
}
