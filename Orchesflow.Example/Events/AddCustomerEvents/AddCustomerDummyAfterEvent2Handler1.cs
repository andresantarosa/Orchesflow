using MediatR;
using Orchesflow.Example.TestExecutionVerifiers;
using Orchesflow.Notifications;
using Orchesflow.Orchestration;

namespace Orchesflow.Example.Events.AddCustomerEvents;

public class AddCustomerDummyAfterEvent2Handler1 : IFallbackable, INotificationHandler<AddCustomerDummyAfterEvent2>
{
    private readonly IDomainNotifications _notifications;

    public AddCustomerDummyAfterEvent2Handler1(IDomainNotifications notifications)
    {
        _notifications = notifications;
    }

    public async Task Handle(AddCustomerDummyAfterEvent2 notification, CancellationToken cancellationToken)
    {
        if (notification.Name == "Def")
        {
            _notifications.AddNotification("Invalid name");
            return;
        }
        TestExecutionVerify.Executions.Add(("AddCustomerDummyAfterEvent2Handler", true));
    }
    
    public async Task Fallback()
    {
        TestExecutionVerify.Executions.Add(("AddCustomerDummyAfterEvent2HandlerFallback", false));
    }
}