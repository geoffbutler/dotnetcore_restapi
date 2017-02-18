using System;
using System.Linq;
using System.Threading.Tasks;

namespace ContactsCore.Data
{
    public interface IContactsUnitOfWork : IDisposable
    {
        IQueryable<TDao> Query<TDao>()
            where TDao : Dao.Base;

        void Add<TDao>(TDao dao)
            where TDao : Dao.Base;

        void Update<TDao>(TDao dao)
            where TDao : Dao.Base;

        void SoftDelete<TDao>(TDao dao)
            where TDao : Dao.Base;

        Task<int> SaveChangesAsync();
    }
}
