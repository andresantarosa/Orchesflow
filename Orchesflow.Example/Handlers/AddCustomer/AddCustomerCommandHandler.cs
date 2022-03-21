using MediatR;
using Orchesflow.Events;
using Orchesflow.Example.Events.AddCustomerEvents;
using Orchesflow.Example.Models;
using Orchesflow.Example.Repository;
using Orchesflow.Example.TestExecutionVerifiers;
using Orchesflow.Notifications;
using Orchesflow.Orchestration;

namespace Orchesflow.Example.Handlers.AddCustomer;

public class AddCustomerCommandHandler : IFallbackable, IRequestHandler<AddCustomerCommand, AddCustomerCommandResponseViewModel>
{
    private readonly ICustomerRepository _repository;
    private readonly IDomainNotifications _notifications;
    private readonly IEventDispatcher _eventDispatcher;

    public AddCustomerCommandHandler(ICustomerRepository repository,
        IDomainNotifications notifications,
        IEventDispatcher eventDispatcher)
    {
        _repository = repository;
        _notifications = notifications;
        _eventDispatcher = eventDispatcher;
    }

    public async Task<AddCustomerCommandResponseViewModel> Handle(AddCustomerCommand request, CancellationToken cancellationToken)
    {
        var response = new AddCustomerCommandResponseViewModel();

        var customer = new Customer()
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Length <= 50 ? request.Name : null
        };

        await _repository.Add(customer);

        _eventDispatcher.AddPreCommitEvent(new AddCustomerDummyPreEvent1(request.Name));
        _eventDispatcher.AddPreCommitEvent(new AddCustomerDummyPreEvent2(request.Name));
        _eventDispatcher.AddPreCommitEvent(new AddCustomerDummyPreEvent3(request.Name));
        
        _eventDispatcher.AddAfterCommitEvent(new AddCustomerDummyAfterEvent1(request.Name));
        _eventDispatcher.AddAfterCommitEvent(new AddCustomerDummyAfterEvent2(request.Name));
        
        return response;
    }

    public async Task Fallback()
    {
        TestExecutionVerify.Executions.Add(("HandlerFallback", false));
    }
}