using MediatR;
using Orchesflow.Example.TestExecutionVerifiers;
using Orchesflow.Notifications;
using Orchesflow.Orchestration;

namespace Orchesflow.Example.Events.AddCustomerEvents;

public class AddCustomerDummyPreEvent1Handler1 : IFallbackable, INotificationHandler<AddCustomerDummyPreEvent1>
{
    private readonly IDomainNotifications _notifications;

    public AddCustomerDummyPreEvent1Handler1(IDomainNotifications notifications)
    {
        _notifications = notifications;
    }

    public async Task Handle(AddCustomerDummyPreEvent1 notification, CancellationToken cancellationToken)
    {
        if (notification.Name == "123")
        {
            _notifications.AddNotification("Invalid name");
            return;
        }
        TestExecutionVerify.Executions.Add(("AddCustomerDummyPreEvent1Handler", true));
    }

    public async Task Fallback()
    {
        TestExecutionVerify.Executions.Add(("AddCustomerDummyPreEvent1Handler", false));
    }
}