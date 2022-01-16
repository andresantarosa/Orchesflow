using System.Collections.Generic;

namespace Orchesflow.Notifications
{
    public interface IDomainNotifications
    {
        void AddNotification(string notification);
        bool HasNotifications();
        List<string> GetAll();
        void CleanNotifications();
    }
}
