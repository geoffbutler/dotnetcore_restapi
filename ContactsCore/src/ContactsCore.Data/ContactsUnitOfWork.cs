using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ContactsCore.Data
{
    public class ContactsUnitOfWork : IContactsUnitOfWork
    {
        private ContactsContext _db;

        public ContactsUnitOfWork(ContactsContext db)
        {
            _db = db;
        }


        private IQueryable<TDao> BaseQuery<TDao>()
            where TDao : Dao.Base
        {
            return _db.Set<TDao>().Where(o => !o.IsDeleted);
        }
         
           
        public IQueryable<TDao> Query<TDao>()
            where TDao : Dao.Base
        {
            return BaseQuery<TDao>();
        }

        public void Add<TDao>(TDao dao)
            where TDao : Dao.Base
        {
            dao.Created = DateTime.UtcNow;

            _db.Set<TDao>().Add(dao);
        }

        public void Update<TDao>(TDao dao)
            where TDao : Dao.Base
        {
            dao.Modified = DateTime.UtcNow;

            _db.Set<TDao>().Attach(dao);
            _db.Entry(dao).State = EntityState.Modified;
        }

        public void SoftDelete<TDao>(TDao dao)
            where TDao : Dao.Base
        {            
            dao.IsDeleted = true;

            Update(dao);            
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync();
        }


        #region IDisposable

        // Flag: Has Dispose already been called?
        private bool _disposed;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                if (_db != null)
                {
                    _db.Dispose();
                    _db = null;
                }
            }

            // Free any unmanaged objects here.
            //            
            _disposed = true;
        }

        ~ContactsUnitOfWork()
        {
            Dispose(false);
        }

        #endregion
    }
}
