using System;
using System.Threading.Tasks;

namespace ContactsCore.Common.Interfaces
{
    public interface ICrudManager<TModel> : IDisposable
        where TModel : IBase
    {        
        Task<ManagerResponse<TModel>> Get(int pageNumber, int pageSize);
        Task<ManagerResponse<TModel>> GetByUid(Guid uid);
        Task<ManagerResponse<TModel>> Create(TModel model);
        Task<ManagerResponse<TModel>> CreateOrUpdate(TModel model);
        Task<ManagerResponse<TModel>> Delete(Guid uid);
    }
}
