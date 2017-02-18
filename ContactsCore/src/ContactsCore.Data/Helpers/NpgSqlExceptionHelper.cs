using ContactsCore.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ContactsCore.Data.Helpers
{
    public class NpgSqlExceptionHelper : IDbExceptionHelper
    {
        public DbUpdateExceptionType HandleUpdateException(DbUpdateException ex)
        {
            var pgEx = ex.InnerException as PostgresException;
            if (pgEx != null)
            {
                switch (pgEx.SqlState)
                {
                    case "23503":
                        return DbUpdateExceptionType.ForeignKeyConstraintViolation;
                    case "23505":
                        return DbUpdateExceptionType.UniqueKeyConstraintViolation;                    
                }
            }
            return DbUpdateExceptionType.Unknown;
        }
    }
}