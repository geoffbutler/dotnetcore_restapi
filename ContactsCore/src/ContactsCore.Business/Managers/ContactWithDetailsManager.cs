using AutoMapper;
using ContactsCore.Common;
using ContactsCore.Data;
using ContactsCore.Data.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContactsCore.Business.Managers
{
    public class ContactWithDetailsManager : IDisposable
    {
        private readonly ILogger<ContactWithDetailsManager> _logger;
        private readonly IMapper _mapper;
        private readonly IDbExceptionHelper _dbExceptionHelper; // TODO
        private IContactsUnitOfWork _unitOfWork;


        public ContactWithDetailsManager(ILogger<ContactWithDetailsManager> logger,
            IMapper mapper, IDbExceptionHelper dbExceptionHelper,
            IContactsUnitOfWork unitOfWork)
        {
            _logger = logger;
            _mapper = mapper;
            _dbExceptionHelper = dbExceptionHelper;
            _unitOfWork = unitOfWork;
        }        

        private IQueryable<Data.Dao.ContactWithDetailsQueryResult> BaseQuery(Guid? contactUid = null) =>
            (
                from c in _unitOfWork.Query<Data.Dao.Contact>()
                where (contactUid == null || c.Uid == contactUid)
                select new Data.Dao.ContactWithDetailsQueryResult
                {
                    Contact = c,
                    Details = c.Details.Where(o => !o.IsDeleted).ToList() // TODO
                }

            ).AsNoTracking();

        public async Task<ManagerResponse<Model.ViewModels.ContactWithDetailsViewModel>> Get(int pageNumber, int pageSize)
        {
            _logger.LogInformation("Get: Begin");
            
            var totalCount = await BaseQuery().LongCountAsync();

            var skip = (pageNumber - 1) * pageSize;
            var daos = await BaseQuery()
                .OrderBy(o => o.Contact.Id)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            var result = new ManagerResponse<Model.ViewModels.ContactWithDetailsViewModel>
            {
                ResultStatus = Common.Enums.ManagerResponseResult.Success,
                Result = new List<Model.ViewModels.ContactWithDetailsViewModel>(),
                PageMeta = new PagingMetadata
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Total = totalCount
                }
            };

            var models = _mapper.Map<List<Model.ViewModels.ContactWithDetailsViewModel>>(daos);
            result.Result.AddRange(models);

            _logger.LogInformation("Get: End");
            return result;
        }

        public async Task<ManagerResponse<Model.ViewModels.ContactWithDetailsViewModel>> GetByUid(Guid uid)
        {
            _logger.LogInformation("GetByUid: Begin");
            
            var result = new ManagerResponse<Model.ViewModels.ContactWithDetailsViewModel>
            {
                ResultStatus = Common.Enums.ManagerResponseResult.Success,
                Result = new List<Model.ViewModels.ContactWithDetailsViewModel>()
            };

            var dao = await BaseQuery(uid)
                .FirstOrDefaultAsync();

            if (dao == null)
            {
                _logger.LogWarning("GetByUid: End (NotFound)");
                result.ResultStatus = Common.Enums.ManagerResponseResult.NotFound;
                return result;
            }

            var model = _mapper.Map<Model.ViewModels.ContactWithDetailsViewModel>(dao);
            result.Result.Add(model);
            
            _logger.LogInformation("GetByUid: End");
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
                if (_unitOfWork != null)
                {
                    _unitOfWork.Dispose();
                    _unitOfWork = null;
                }
            }

            // Free any unmanaged objects here.
            //            
            _disposed = true;
        }

        ~ContactWithDetailsManager()
        {
            Dispose(false);
        }

        #endregion
    }
}
