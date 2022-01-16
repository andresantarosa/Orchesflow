using System.Threading.Tasks;

namespace Orchesflow.UnitOfWork
{
    public interface IUnitOfWork<T>
    {
        public Task<bool> Commit();
    }
}
