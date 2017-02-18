using System;
using ContactsCore.Common;
using ContactsCore.Common.Interfaces;
using ContactsCore.Model.Models;
using Microsoft.Extensions.Logging;
using AutoMapper;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using ContactsCore.Data.Helpers;
using ContactsCore.Data;

namespace ContactsCore.Business.Managers
{
    public class ContactsManager : ICrudManager<Contact>
    {
        private readonly ILogger<ContactsManager> _logger;        
        private readonly IMapper _mapper;
        private readonly IDbExceptionHelper _dbExceptionHelper;
        private IContactsUnitOfWork _unitOfWork;
        

        public ContactsManager(ILogger<ContactsManager> logger,
            IMapper mapper, IDbExceptionHelper dbExceptionHelper,
            IContactsUnitOfWork unitOfWork)
        {
            _logger = logger;            
            _mapper = mapper;
            _dbExceptionHelper = dbExceptionHelper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ManagerResponse<Contact>> Get(int pageNumber, int pageSize)
        {
            _logger.LogInformation("Get: Begin");

            var totalCount = await _unitOfWork.Query<Data.Dao.Contact>()
                .LongCountAsync();

            var skip = (pageNumber - 1) * pageSize;
            var daos = await _unitOfWork.Query<Data.Dao.Contact>()
                .AsNoTracking()
                .OrderBy(o => o.Id)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            var models = _mapper.Map<IEnumerable<Contact>>(daos)
                .ToList();

            var result = new ManagerResponse<Contact>
            {
                ResultStatus = Common.Enums.ManagerResponseResult.Success, 
                Result = new List<Contact>(), 
                PageMeta = new PagingMetadata
                {
                    PageNumber = pageNumber, 
                    PageSize = pageSize, 
                    Total = totalCount
                }
            };
            result.Result.AddRange(models);

            _logger.LogInformation("Get: End");

            return result;
        }

        public async Task<ManagerResponse<Contact>> GetByUid(Guid uid)
        {
            _logger.LogInformation("GetByUid: Begin");

            var dao = await _unitOfWork.Query<Data.Dao.Contact>()
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Uid == uid);

            var result = new ManagerResponse<Contact>
            {
                ResultStatus = Common.Enums.ManagerResponseResult.Success,
                Result = new List<Contact>()
            };

            if (dao == null)
            {
                result.ResultStatus = Common.Enums.ManagerResponseResult.NotFound;

                _logger.LogWarning("GetByUid: End (Not Found)");
                return result;
            }

            var model = _mapper.Map<Contact>(dao);
            result.Result.Add(model);

            _logger.LogInformation("GetByUid: End");

            return result;
        }

        public async Task<ManagerResponse<Contact>> Create(Contact model)
        {
            _logger.LogInformation("Create: Begin");

            var dao = _mapper.Map<Contact, Data.Dao.Contact>(model);
            _unitOfWork.Add(dao);

            var result = new ManagerResponse<Contact>
            {
                ResultStatus = Common.Enums.ManagerResponseResult.Success,
                Result = new List<Contact>()
            };

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError("Create: DbUpdateException", ex);
                
                var dbErrorType = _dbExceptionHelper.HandleUpdateException(ex);
                switch (dbErrorType)
                {
                    case Common.Enums.DbUpdateExceptionType.UniqueKeyConstraintViolation:
                        result.ResultStatus = Common.Enums.ManagerResponseResult.UniqueKeyViolation;
                        result.ErrorMessage = "Unique constraint violation";
                        return result;
                    default:
                        result.ResultStatus = Common.Enums.ManagerResponseResult.UnknownError;
                        result.ErrorMessage = "Unknown database error";
                        return result;
                }
            }
            // TODO

            var createdModel = _mapper.Map<Data.Dao.Contact, Contact>(dao);
            result.Result.Add(createdModel);

            _logger.LogInformation("Create: End");
            return result;
        }

        public async Task<ManagerResponse<Contact>> CreateOrUpdate(Contact model)
        {
            _logger.LogInformation("CreateOrUpdate: Begin");

            var created = false;
            var dao = await _unitOfWork.Query<Data.Dao.Contact>()
                .FirstOrDefaultAsync(o => o.Uid == model.Uid);
            if (dao == null)
            {
                // create
                dao = _mapper.Map<Contact, Data.Dao.Contact>(model);
                _unitOfWork.Add(dao);
                created = true;
            }
            else
            {
                // update
                dao.FirstName = model.FirstName;
                dao.LastName = model.LastName;
                _unitOfWork.Update(dao);
            }

            var result = new ManagerResponse<Contact>
            {
                ResultStatus = Common.Enums.ManagerResponseResult.Success,
                Result = new List<Contact>()
            };

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {// TODO
                var dbErrorType = _dbExceptionHelper.HandleUpdateException(ex);
                switch (dbErrorType)
                {
                    case Common.Enums.DbUpdateExceptionType.UniqueKeyConstraintViolation:
                        result.ResultStatus = Common.Enums.ManagerResponseResult.UniqueKeyViolation;
                        result.ErrorMessage = "Unique constraint violation";
                        return result;
                    default:
                        result.ResultStatus = Common.Enums.ManagerResponseResult.UnknownError;
                        result.ErrorMessage = "Unknown database error";
                        return result;
                }
            }            

            var createdOrUpdatedModel = _mapper.Map<Data.Dao.Contact, Contact>(dao);
            result.Result.Add(createdOrUpdatedModel);
            if (created)
            {
                _logger.LogInformation("CreateOrUpdate: End (Created)");
                result.ResultStatus = Common.Enums.ManagerResponseResult.Created;
                return result;
            }
            else
            { // updated
                _logger.LogInformation("CreateOrUpdate: End (Updated)");
                result.ResultStatus = Common.Enums.ManagerResponseResult.Updated;
                return result;
            }
        }

        public async Task<ManagerResponse<Contact>> Delete(Guid uid)
        {
            _logger.LogInformation("Delete: Begin");

            var dao = await _unitOfWork.Query<Data.Dao.Contact>()
                .FirstOrDefaultAsync(o => o.Uid == uid);

            var result = new ManagerResponse<Contact>
            {
                ResultStatus = Common.Enums.ManagerResponseResult.Success,
                Result = new List<Contact>()
            };

            if (dao == null)
            {
                _logger.LogWarning("Delete: End (NotFound)");
                result.ResultStatus = Common.Enums.ManagerResponseResult.NotFound;
                return result;
            }

            _unitOfWork.SoftDelete(dao);

            await _unitOfWork.SaveChangesAsync();
            // TODO

            _logger.LogInformation("Delete: End");
            result.ResultStatus = Common.Enums.ManagerResponseResult.Success;
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

        ~ContactsManager()
        {
            Dispose(false);
        }

        #endregion
    }
}
