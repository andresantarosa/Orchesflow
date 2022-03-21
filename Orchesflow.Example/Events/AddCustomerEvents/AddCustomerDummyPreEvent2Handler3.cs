using MediatR;
using Orchesflow.Example.TestExecutionVerifiers;
using Orchesflow.Notifications;
using Orchesflow.Orchestration;

namespace Orchesflow.Example.Events.AddCustomerEvents;

public class AddCustomerDummyPreEvent3Handler1 : INotificationHandler<AddCustomerDummyPreEvent3>
{
    private readonly IDomainNotifications _notifications;

    public AddCustomerDummyPreEvent3Handler1(IDomainNotifications notifications)
    {
        _notifications = notifications;
    }

    public async Task Handle(AddCustomerDummyPreEvent3 notification, CancellationToken cancellationToken)
    {
        TestExecutionVerify.Executions.Add(("AddCustomerDummyPreEvent3Handler", true));
    }
}