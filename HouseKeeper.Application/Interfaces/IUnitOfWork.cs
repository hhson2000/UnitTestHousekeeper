using System.Linq;

namespace HouseKeeper.Application
{
    public interface IUnitOfWork
    {
        IQueryable<T> Query<T>();
    }
}