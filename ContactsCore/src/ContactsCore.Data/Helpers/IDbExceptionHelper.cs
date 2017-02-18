using ContactsCore.Common.Enums;
using Microsoft.EntityFrameworkCore;

namespace ContactsCore.Data.Helpers
{
    public interface IDbExceptionHelper
    {
        DbUpdateExceptionType HandleUpdateException(DbUpdateException ex);
    }
}
