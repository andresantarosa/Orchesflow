using MediatR;
using Orchesflow.Example.TestExecutionVerifiers;
using Orchesflow.Notifications;
using Orchesflow.Orchestration;

namespace Orchesflow.Example.Events.AddCustomerEvents;

public class AddCustomerDummyAfterEvent1Handler1 : IFallbackable, INotificationHandler<AddCustomerDummyAfterEvent1>
{
    private readonly IDomainNotifications _notifications;

    public AddCustomerDummyAfterEvent1Handler1(IDomainNotifications notifications)
    {
        _notifications = notifications;
    }

    public async Task Handle(AddCustomerDummyAfterEvent1 notification, CancellationToken cancellationToken)
    {
        if (notification.Name == "456")
        {
            _notifications.AddNotification("Invalid name");
            return;
        }
        TestExecutionVerify.Executions.Add(("AddCustomerDummyAfterEvent1Handler", true));
    }

    public async Task Fallback()
    {
        TestExecutionVerify.Executions.Add(("AddCustomerDummyAfterEvent1Handler", false));
    }
}