# Orchesflow

This package provides some extra tools to be used along MediatR + EntityFramework focused to make working with events easier.

#### Ochestrator flow:

![Orchesflow diagram](https://i.imgur.com/ffI6xD1.png)

### Using Ochesflow
To enable Orchesflow just add it to your `ConfigureServices()` method at `Startup.cs`

    services.AddOrchesflow<YourDbContext>();
    
#### Sending a Command or Query

    using Orchesflow.Orchestration
    
    public class MyController
    {
       private readonly IOrchestrator _orchestrator;
       
       public MyController(IOrchestrator orchestrator)
       {
           _orchestrator = orchestrator
       }
       
       public async Task<IActionResult>([FromBody] MyCommand command)
       {
           // Use SendCommand to send a command
           // A command triggers PreCommitEvents, Database commit and AfterCommitEvents
           // For Queries the method SendQuery() should be used
           // SendQuery() does not trigger events nor database commit
           var response = await _orchestrator.SendCommand(command);
           if(respose.Success)
              return Ok(response.Data);
           else
              return BadRequest(response.Messages);
       }
    }

### EventDispatcher
The event dispatcher is the responsible to dispatch PreCommitEvents and AfterCommitEvents. This interface (`IEventDispatcher`) provides methods to add, remove, list or manualy event trigger (not recommended).
##### Available methods
`GetPreCommitEvent(INotification evt)` => Returns a list with all PreCommitEvent
`AddPreCommitEvent(INotification evt)` => Adds new PreCommitEvent
`RemovePreCommitEvent(INotification evt)` => Remove an existing PreCommitEvent
`FirePreCommitEvents(INotification evt)` => Manualy fire PreCommitEvent (not recommended)

`GetAfterCommitEvent(INotification evt)` => Returns a list with all AfterCommitEvent
`AddAfterCommitEvent(INotification evt)` => Adds new AfterCommitEvent
`RemoveAfterCommitEvent(INotification evt)` => Remove an existing AfterCommitEvent
`FireAfterCommitEvents(INotification evt)` => Manualy fire AfterCommitEvent (not recommended)

The EventDispatcher functionality is only available when usind `SendCommand()`

#### Commit
The `SaveChanges()` method is called whenever the MediatR CommandHandler finishes its job, unless if any notification is found at the DomainNotifications container.
The Commit will only be evoked when using `SendCommand()`

#### PreCommitEvents
PreCommitEvents are MediatR events (`INotification`) that will be triggered before commit occurs. An unlimited number of PreCommitEvents can be added to EventDispatcher.
To add a PreCommitEvent use `IEventDispatcher.AddPreCommitEvents()`

#### AfterCommitEvents
AfterCommitEvents are MediatR events (`INotification`) that will be triggered after the  commit occurs and if no notification is found at DomainNotifications container.. An unlimited number of PreCommitEvents can be added to EventDispatcher.

#### DomainNotifications container
This container intention is to store any errors that occurs during the request lifecycle and provide easy access to them from any part of your code. It is important to note that Commit action, AfterCommitEvents and the flow return depends if there is or there is not messages at DomainNotifications container. This funcionality should be used whenever you want to prevent the Commit to happen or prevent AfterCommitEvents to be triggered.

### Fallbacks

Sometimes things just go wrong, in this case you can use fallbacks to go back and undo what you did.
To enable fallbacks, implement the interface `IFallbackable` at your RequestHandler or Notification Handler and put your logic inside `Fallback()` method.

There are three key moments for fallbacks.

- **An error occurred during a PreCommitEvent**: All PreCommitEvents already executed will have its fallback method called in the reverse order they were executed

- **An error occurred during Commit phase:** The calling handler will have its `Fallback` method triggered and also all PreCommitEvents executed will have its fallback method called in the reverse order they were executed

- **An error occurred during a AfterCommitEvent:** All PreCommitEvents already executed will have its `Fallback` method triggered, Handler fallback and PreCommitEvents fallbacks will also be triggered in the reverse worder they were executed.