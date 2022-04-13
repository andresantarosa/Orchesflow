using MediatR;
using Orchesflow.Example.TestExecutionVerifiers;
using Orchesflow.Notifications;
using Orchesflow.Orchestration;

namespace Orchesflow.Example.Events.AddCustomerEvents;

public class AddCustomerDummyPreEvent2Handler1 : IFallbackable, INotificationHandler<AddCustomerDummyPreEvent2>
{
    private readonly IDomainNotifications _notifications;

    public AddCustomerDummyPreEvent2Handler1(IDomainNotifications notifications)
    {
        _notifications = notifications;
    }

    public async Task Handle(AddCustomerDummyPreEvent2 notification, CancellationToken cancellationToken)
    {
        if (notification.Name == "Abc")
        {
            _notifications.AddNotification("Invalid name");
            return;
        }
        TestExecutionVerify.Executions.Add(("AddCustomerDummyPreEvent2Handler", true));
    }
    
    public async Task Fallback()
    {
        TestExecutionVerify.Executions.Add(("AddCustomerDummyPreEvent2HandlerFallback", false));
    }
}