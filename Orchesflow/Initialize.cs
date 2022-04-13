using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Orchesflow.Events;
using Orchesflow.Notifications;
using Orchesflow.Orchestration;
using Orchesflow.UnitOfWork;

namespace Orchesflow
{
    public static class Initialize
    {
        public static IServiceCollection AddOrchesflow<TDbContext>(this IServiceCollection serviceCollection) where TDbContext : DbContext
        {
            serviceCollection.AddScoped<IEventDispatcher, EventDispatcher>();
            serviceCollection.AddScoped<IOrchestrator, Orchestrator<TDbContext>>();
            serviceCollection.AddScoped<IDomainNotifications, DomainNotifications>();
            serviceCollection.AddScoped<IUnitOfWork<TDbContext>, UnitOfWork<TDbContext>>();
            return serviceCollection;
        }
    }
}
