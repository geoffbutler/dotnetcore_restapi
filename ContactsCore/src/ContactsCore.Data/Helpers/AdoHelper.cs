using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Threading.Tasks;

namespace ContactsCore.Data.Helpers
{
    public class AdoHelper : IAdoHelper
    {
        public DbConnection GetConnection(DbContext context)
        {
            return context.Database.GetDbConnection();
        }

        public DbCommand CreateCommand(DbConnection connection)
        {
            return connection.CreateCommand();
        }

        public async Task<object> ExecuteScalarAsync(DbCommand command)
        {
            return await command.ExecuteScalarAsync();
        }

        public async Task OpenConnectionAsync(DbConnection connection)
        {
            await connection.OpenAsync();
        }
    }
}
