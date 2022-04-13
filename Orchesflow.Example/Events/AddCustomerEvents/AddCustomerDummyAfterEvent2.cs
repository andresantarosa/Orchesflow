using MediatR;

namespace Orchesflow.Example.Events.AddCustomerEvents;

public class AddCustomerDummyAfterEvent2 : INotification
{
    public AddCustomerDummyAfterEvent2(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
}