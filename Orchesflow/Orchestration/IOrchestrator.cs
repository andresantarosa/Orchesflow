using MediatR;
using Orchesflow.Models;
using System.Threading.Tasks;

namespace Orchesflow.Orchestration
{
    public interface IOrchestrator
    {
        Task<RequestResult<TResponse>> SendCommand<TRequest, TResponse>(IRequest<TResponse> request)
            where TRequest : IRequest<TResponse>;
        Task<RequestResult<TResponse>> SendQuery<TResponse>(IRequest<TResponse> request);
    }
}
