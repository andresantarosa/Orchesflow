using MediatR;

namespace Orchesflow.Example.Events.AddCustomerEvents;

public class AddCustomerDummyPreEvent1 : INotification
{
    public AddCustomerDummyPreEvent1(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
}