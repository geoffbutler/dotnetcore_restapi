using System;
using ContactsCore.Model.ViewModels;
using Microsoft.Extensions.Logging;
using ContactsCore.Data;
using System.Threading.Tasks;
using System.Reflection;
using ContactsCore.Data.Helpers;

namespace ContactsCore.Business.Managers
{
    public class HealthcheckManager : IDisposable
    {
        private readonly ILogger<HealthcheckManager> _logger;
        private readonly IAdoHelper _adoHelper;
        private ContactsContext _db;        

        public HealthcheckManager(ILogger<HealthcheckManager> logger,             
            ContactsContext db, IAdoHelper adoHelper)
        {
            _logger = logger;
            _db = db;
            _adoHelper = adoHelper;
        }

        public async Task<HealthcheckViewModel> Get()
        {
            _logger.LogInformation("Get: Begin");
                        
            var result = new HealthcheckViewModel
            {
                ApiVersion = Assembly.GetEntryAssembly().GetName().Version.ToString(),
                DbStatus = "OK"
            };

            try
            {
                using (var connection = _adoHelper.GetConnection(_db))
                {
                    await _adoHelper.OpenConnectionAsync(connection);
                
                    using (var command = _adoHelper.CreateCommand(connection))
                    {
                        command.CommandText = "SELECT COUNT(*) FROM \"__EFMigrationsHistory\"";
                        await _adoHelper.ExecuteScalarAsync(command);
                    }
                }
            }
            catch (Exception ex)
            {                
                _logger.LogError("Get: Exception", ex);
                result.DbStatus = "ERROR";                
            }

            _logger.LogInformation("Get: End");

            return result;
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

            if (disposing) {
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

        ~HealthcheckManager()
        {
            Dispose(false);
        }

        #endregion
    }
}
