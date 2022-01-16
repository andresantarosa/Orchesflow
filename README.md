# Orchesflow

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=andresantarosa_Orchesflow&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=andresantarosa_Orchesflow)

This package provides some extra tools to be used along MediatR + EntityFramework focused to make working with events easier.

#### Ochestrator flow:

![Orchesflow diagram](https://i.imgur.com/ffI6xD1.png)

### Using Ochesflow
To enable Orchesflow just add it to your `ConfigureServices()` method at `Startup.cs`

    services.AddOchesflow<YourDbContext>();
    
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

#### EventDispatcher
The event dispatcher is the responsible to dispatch PreCommitEvents and AfterCommitEvents. This interface (`IEventDispatcher`) provides methods to add, remove, list or manualy event trigger (not recommended).
##### Available methods for PreCommitEvents
`GetPreCommitEvent(INotification evt)` => Returns a list with all PreCommitEvent

`AddPreCommitEvent(INotification evt)` => Adds new PreCommitEvent

`RemovePreCommitEvent(INotification evt)` => Remove an existing PreCommitEvent

`FirePreCommitEvents(INotification evt)` => Manualy fire PreCommitEvent (not recommended)

####  
##### Available methods for PreCommitEvents


`GetAfterCommitEvent(INotification evt)` => Returns a list with all AfterCommitEvent

`AddAfterCommitEvent(INotification evt)` => Adds new AfterCommitEvent

`RemoveAfterCommitEvent(INotification evt)` => Remove an existing AfterCommitEvent

`FireAfterCommitEvents(INotification evt)` => Manualy fire AfterCommitEvent (not recommended)


The EventDispatcher functionality is only available when using `SendCommand()`

#### Commit
`SaveChanges()` method is called whenever MediatR CommandHandler finishes its job, unless if any notification is found at the DomainNotifications container.
The Commit will only be evoked when using `SendCommand()`

#### PreCommitEvents
PreCommitEvents are MediatR events (`INotification`) that will be triggered before a commit occurs. An unlimited number of PreCommitEvents can be added to EventDispatcher.
To add a PreCommitEvent use `IEventDispatcher.AddPreCommitEvents()`

#### AfterCommitEvents
AfterCommitEvents are MediatR events (`INotification`) that will be triggered after the  commit occurs and if no notification is found at DomainNotifications container. An unlimited number of PreCommitEvents can be added to EventDispatcher.

#### DomainNotifications container
This container intention is to store any errors that occurs during the request lifecycle and provide easy access to them from any part of your code. It is important to note that Commit action, AfterCommitEvents and the flow return depends if there is or there is not messages at DomainNotifications container. This funcionality should be used whenever you want to prevent the Commit to happen or prevent AfterCommitEvents to be triggered.

##### Available methods for IDomainNotifications interface
`AddNotification(string notification)` => Adds a new notification to container

`void CleanNotifications()` => Remove all notifications from container

`List<string> GetAll()` => Returns a list with all notifications

`HasNotifications()` => Returns true if any notifications is found at container or false if not 

