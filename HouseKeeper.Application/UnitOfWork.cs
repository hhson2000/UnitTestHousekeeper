using System.Collections.Generic;
using System.Linq;

namespace HouseKeeper.Application
{
    public class UnitOfWork : IUnitOfWork
    {
        public IQueryable<T> Query<T>()
        {
            return new List<T>().AsQueryable();
        }
    }
}