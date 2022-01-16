using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchesflow.Events
{
    public interface IEventDispatcher
    {
        List<INotification> GetPreCommitEvents();
        void AddPreCommitEvent(INotification evt);
        void RemovePreCommitEvent(INotification evt);
        Task FirePreCommitEvents();


        List<INotification> GetAfterCommitEvents();
        void AddAfterCommitEvent(INotification evt);
        void RemoveAfterCommitEvent(INotification evt);
        Task FireAfterCommitEvents();
    }
}
