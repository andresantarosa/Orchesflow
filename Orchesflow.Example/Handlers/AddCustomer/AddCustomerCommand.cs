using MediatR;

namespace Orchesflow.Example.Handlers.AddCustomer;

public class AddCustomerCommand : IRequest<AddCustomerCommandResponseViewModel>
{
    public string Name { get; set; }
}