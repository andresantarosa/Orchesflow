using MediatR;

namespace Orchesflow.Example.Events.AddCustomerEvents;

public class AddCustomerDummyPreEvent2 : INotification
{
    public AddCustomerDummyPreEvent2(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
}