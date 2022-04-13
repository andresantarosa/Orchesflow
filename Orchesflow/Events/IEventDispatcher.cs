using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchesflow.Orchestration;

namespace Orchesflow.Events
{
    public interface IEventDispatcher
    {
        List<INotification> GetPreCommitEvents();
        void AddPreCommitEvent(INotification evt);
        void RemovePreCommitEvent(INotification evt);
        Task FirePreCommitEvents();
        Task FirePreCommitFallbacks();


        List<INotification> GetAfterCommitEvents();
        void AddAfterCommitEvent(INotification evt);
        void RemoveAfterCommitEvent(INotification evt);
        Task FireAfterCommitEvents();
        Task FireAfterCommitFallbacks();

        
        
    }
}
