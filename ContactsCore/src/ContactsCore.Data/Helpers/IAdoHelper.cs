using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Threading.Tasks;

namespace ContactsCore.Data.Helpers
{
    public interface IAdoHelper
    {
        DbConnection GetConnection(DbContext context);
        DbCommand CreateCommand(DbConnection connection);
        Task OpenConnectionAsync(DbConnection connection);
        Task<object> ExecuteScalarAsync(DbCommand command);
    }
}
