using MediatR;

namespace Orchesflow.Example.Events.AddCustomerEvents;

public class AddCustomerDummyAfterEvent1 : INotification
{
    public AddCustomerDummyAfterEvent1(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
}