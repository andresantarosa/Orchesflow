using Microsoft.EntityFrameworkCore;
using Orchesflow.Notifications;
using System;
using System.Threading.Tasks;

namespace Orchesflow.UnitOfWork
{
    public sealed class UnitOfWork<T> : IUnitOfWork<T> where T : DbContext
    {
        private readonly T _context;
        private readonly IDomainNotifications _domainNotifications;
        public UnitOfWork(T context, IDomainNotifications domainNotifications)
        {
            _context = context;
            _domainNotifications = domainNotifications;
        }

        public async Task<bool> Commit()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _domainNotifications.AddNotification($"An error ocurred while saving data {e.Message}");
                return false;
            }
        }
    }
}
