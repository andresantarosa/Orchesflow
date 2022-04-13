using System;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Orchesflow.Notifications;
using Orchesflow.Orchestration;

namespace Orchesflow.Events
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly IMediator _mediator;
        private readonly IDomainNotifications _domainNotifications;
        private readonly IServiceProvider _serviceProvider;

        public EventDispatcher(IMediator mediator, IDomainNotifications domainNotifications,
            IServiceProvider serviceProvider)
        {
            _mediator = mediator;
            _domainNotifications = domainNotifications;
            _serviceProvider = serviceProvider;
        }

        private List<INotification> _preCommitEvents { get; } = new List<INotification>();
        private List<IFallbackable> _preCommitFallbacks { get; } = new List<IFallbackable>();

        private List<INotification> _afterCommitEvents { get; } = new List<INotification>();
        private List<IFallbackable> _afterCommitFallbacks { get; } = new List<IFallbackable>();


        public List<INotification> GetPreCommitEvents()
        {
            return _preCommitEvents;
        }

        public void AddPreCommitEvent(INotification evt)
        {
            _preCommitEvents.Add(evt);
        }

        public void RemovePreCommitEvent(INotification evt)
        {
            _preCommitEvents.Remove(evt);
        }

        public async Task FirePreCommitEvents()
        {
            foreach (var evt in _preCommitEvents)
            {
                await _mediator.Publish(evt);
                if (_domainNotifications.HasNotifications())
                    break;

                _preCommitFallbacks.AddRange(GetFallbacks(evt));
            }
        }

        public async Task FirePreCommitFallbacks()
        {
            _preCommitFallbacks.Reverse();
            foreach (var fallback in _preCommitFallbacks)
                await fallback.Fallback();
        }


        
        public List<INotification> GetAfterCommitEvents()
        {
            return _afterCommitEvents;
        }

        public void AddAfterCommitEvent(INotification evt)
        {
            _afterCommitEvents.Add(evt);
        }

        public void RemoveAfterCommitEvent(INotification evt)
        {
            _afterCommitEvents.Remove(evt);
        }

        public async Task FireAfterCommitEvents()
        {
            foreach (var evt in _afterCommitEvents)
            {
                await _mediator.Publish(evt);
                if (_domainNotifications.HasNotifications())
                    break;

                _afterCommitFallbacks.AddRange(GetFallbacks(evt));
            }
        }

        public async Task FireAfterCommitFallbacks()
        {
            _afterCommitFallbacks.Reverse();
            foreach (var fallback in _afterCommitFallbacks)
                await fallback.Fallback();
        }


        private List<IFallbackable> GetFallbacks(INotification notification)
        {
            var notificationType = typeof(INotificationHandler<>).MakeGenericType(notification.GetType());
            var notifications = _serviceProvider.GetServices(notificationType);
            var fallbackEvents = notifications
                .OfType<IFallbackable>().ToList();
            return fallbackEvents;
        }
    }
}