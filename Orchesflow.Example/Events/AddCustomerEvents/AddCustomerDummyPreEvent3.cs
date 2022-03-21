using MediatR;

namespace Orchesflow.Example.Events.AddCustomerEvents;

public class AddCustomerDummyPreEvent3 : INotification
{
    public AddCustomerDummyPreEvent3(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
}