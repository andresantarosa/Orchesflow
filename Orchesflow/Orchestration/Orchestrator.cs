using System;
using System.Collections.Generic;
using MediatR;
using Orchesflow.Events;
using Orchesflow.Models;
using Orchesflow.Notifications;
using Orchesflow.UnitOfWork;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Orchesflow.Orchestration
{
    public class Orchestrator<TDbContext> : IOrchestrator
    {
        private readonly IUnitOfWork<TDbContext> _unitOfWork;
        private readonly IDomainNotifications _domainNotifications;
        private readonly IEventDispatcher _eventDispatcher;
        protected readonly IMediator _mediator;
        private readonly IServiceProvider _serviceProvider;
        private readonly List<IFallbackable> _fallbacks; 

        public Orchestrator(IUnitOfWork<TDbContext> unitOfWork,
            IDomainNotifications domainNotifications,
            IEventDispatcher eventDispatcher,
            IMediator mediator,
            IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _domainNotifications = domainNotifications;
            _eventDispatcher = eventDispatcher;
            _mediator = mediator;
            _serviceProvider = serviceProvider;
            _fallbacks = new List<IFallbackable>();
        }

        public async Task<RequestResult> SendCommand<TRequest, TResponse>(IRequest<TResponse> request)
            where TRequest : IRequest<TResponse>
        {
            var handler = _serviceProvider
                .GetService<IRequestHandler<TRequest, TResponse>>();
            
                
            var commandResponse = await _mediator.Send(request);

            // Fire pre commit events
            await _eventDispatcher.FirePreCommitEvents();
            if (_domainNotifications.HasNotifications())
            {
                await _eventDispatcher.FirePreCommitFallbacks();
                return GetRequestResultForFailure();

            }
           
            if (await _unitOfWork.Commit())
            {
                // Fire after commit events
                await _eventDispatcher.FireAfterCommitEvents();

                if (_domainNotifications.HasNotifications())
                {
                    await _eventDispatcher.FireAfterCommitFallbacks();
                    if (handler is IFallbackable)
                        await ((IFallbackable) handler).Fallback();
                    await _eventDispatcher.FirePreCommitFallbacks();
                    return GetRequestResultForFailure();
                }

                return new RequestResult
                {
                    Success = true,
                    Data = commandResponse
                };
            }

            await _eventDispatcher.FirePreCommitFallbacks();
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
