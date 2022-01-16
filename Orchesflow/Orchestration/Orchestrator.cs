using MediatR;
using Orchesflow.Events;
using Orchesflow.Models;
using Orchesflow.Notifications;
using Orchesflow.UnitOfWork;
using System.Threading.Tasks;

namespace Orchesflow.Orchestration
{
    public class Orchestrator<TDbContext> : IOrchestrator
    {
        private readonly IUnitOfWork<TDbContext> _unitOfWork;
        private readonly IDomainNotifications _domainNotifications;
        private readonly IEventDispatcher _eventDispatcher;
        protected readonly IMediator _mediator;

        public Orchestrator(IUnitOfWork<TDbContext> unitOfWork, IDomainNotifications domainNotifications, IEventDispatcher eventDispatcher, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _domainNotifications = domainNotifications;
            _eventDispatcher = eventDispatcher;
            _mediator = mediator;
        }

        public async Task<RequestResult> SendCommand<T>(IRequest<T> request)
        {
            var commandResponse = await _mediator.Send(request);

            // Fire pre commit events
            await _eventDispatcher.FirePreCommitEvents();

            if (_domainNotifications.HasNotifications())
                return GetRequestResultForFailure();

            if (await _unitOfWork.Commit())
            {
                // Fire after commit events
                await _eventDispatcher.FireAfterCommitEvents();

                return new RequestResult
                {
                    Success = true,
                    Data = commandResponse
                };
            }

            return GetRequestResultForFailure();
        }

        public async Task<RequestResult> SendQuery<T>(IRequest<T> request)
        {
            var commandResponse = await _mediator.Send(request);

            if (_domainNotifications.HasNotifications())
                return GetRequestResultForFailure();

            return new RequestResult
            {
                Success = true,
                Data = commandResponse
            };

        }

        private RequestResult GetRequestResultForFailure() =>
            new RequestResult
            {
                Success = false,
                Messages = _domainNotifications.GetAll()
            };

    }
}
