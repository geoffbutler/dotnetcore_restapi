using ContactsCore.Common.Interfaces;
using System;
using System.Threading.Tasks;
using ContactsCore.Common;
using Microsoft.Extensions.Logging;
using AutoMapper;
using ContactsCore.Data.Helpers;
using ContactsCore.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using ContactsCore.Model.Models;

namespace ContactsCore.Business.Managers
{
    public class ContactDetailsManager : ICrudManager<ContactDetail>
    {
        private readonly ILogger<ContactDetailsManager> _logger;
        private readonly IMapper _mapper;
        private readonly IDbExceptionHelper _dbExceptionHelper;
        private IContactsUnitOfWork _unitOfWork;

        public Guid ContactUid { get; private set; }

        public ContactDetailsManager(ILogger<ContactDetailsManager> logger,
            IMapper mapper, IDbExceptionHelper dbExceptionHelper,
            IContactsUnitOfWork unitOfWork)
        {
            _logger = logger;
            _mapper = mapper;
            _dbExceptionHelper = dbExceptionHelper;
            _unitOfWork = unitOfWork;
        }


        public void Init(Guid contactUid)
        {
            ContactUid = contactUid;
        }

        private IQueryable<Data.Dao.Contact> ParentQuery(Guid contactUid) =>
            _unitOfWork.Query<Data.Dao.Contact>()
                .Where(o => o.Uid == contactUid);

        private async Task<long?> TryGetParentId(Guid contactUid) =>
            await ParentQuery(contactUid)
                .Select(o => (long?)o.Id)
                .SingleOrDefaultAsync();


        public async Task<ManagerResponse<ContactDetail>> Get(int pageNumber, int pageSize)
        {
            _logger.LogInformation("Get: Begin");

            var result = new ManagerResponse<ContactDetail>
            {
                ResultStatus = Common.Enums.ManagerResponseResult.Success,
                Result = new List<ContactDetail>(),
                PageMeta = new PagingMetadata
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                }
            };

            var contactId = await TryGetParentId(ContactUid);
            if (contactId == null)
            {
                _logger.LogWarning("Get: End (NotFound)");
                result.ResultStatus = Common.Enums.ManagerResponseResult.NotFound;
                return result;
            }


            var totalCount = await _unitOfWork.Query<Data.Dao.ContactDetail>()
                .Where(o => o.ContactId == contactId)
                .LongCountAsync();
            result.PageMeta.Total = totalCount;

            var skip = (pageNumber - 1) * pageSize;
            var daos = await _unitOfWork.Query<Data.Dao.ContactDetail>()
                .AsNoTracking()
                .Where(o => o.ContactId == contactId)
                .OrderBy(o => o.Id)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            var models = _mapper.Map<IEnumerable<ContactDetail>>(daos)
                .ToList();
            //foreach (var m in models)
            //    m.ContactUid = contactUid;

            result.Result.AddRange(models);

            _logger.LogInformation("Get: End");
            return result;
        }

        public async Task<ManagerResponse<ContactDetail>> GetByUid(Guid uid)
        {
            _logger.LogInformation("GetByUid: Begin");

            var result = new ManagerResponse<ContactDetail>
            {
                ResultStatus = Common.Enums.ManagerResponseResult.Success,
                Result = new List<ContactDetail>()
            };

            var dao = await _unitOfWork.Query<Data.Dao.ContactDetail>()
                .Where(o => o.Contact.Uid == ContactUid)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Uid == uid);
            if (dao == null)
            {
                _logger.LogWarning("GetByUid: End (NotFound)");
                result.ResultStatus = Common.Enums.ManagerResponseResult.NotFound;
                return result;
            }

            var model = _mapper.Map<ContactDetail>(dao);
            //model.ContactUid = ContactUid;
            result.Result.Add(model);

            _logger.LogInformation("GetByUid: End");
            return result;
        }

        public async Task<ManagerResponse<ContactDetail>> Create(ContactDetail model)
        {
            _logger.LogInformation("Create: Begin");

            var result = new ManagerResponse<ContactDetail>
            {
                ResultStatus = Common.Enums.ManagerResponseResult.Success,
                Result = new List<ContactDetail>()
            };

            var dao = _mapper.Map<ContactDetail, Data.Dao.ContactDetail>(model);

            var parentId = await TryGetParentId(ContactUid);
            if (parentId == null)
            {
                _logger.LogWarning("Create: End (Invalid ContactUid)");
                result.ResultStatus = Common.Enums.ManagerResponseResult.ForeignKeyViolation;
                return result;
            }
            dao.ContactId = parentId.Value;

            _unitOfWork.Add(dao);
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
                    case Common.Enums.DbUpdateExceptionType.ForeignKeyConstraintViolation:
                        result.ResultStatus = Common.Enums.ManagerResponseResult.ForeignKeyViolation;
                        result.ErrorMessage = "Foreign key constraint violation";
                        return result;
                    case Common.Enums.DbUpdateExceptionType.UniqueKeyConstraintViolation:
                        result.ResultStatus = Common.Enums.ManagerResponseResult.UniqueKeyViolation;
                        result.ErrorMessage = "Unique key constraint violation";
                        return result;
                    default:
                        result.ResultStatus = Common.Enums.ManagerResponseResult.UnknownError;
                        result.ErrorMessage = "Unknown database error";
                        return result;
                }
            }

            var createdModel = _mapper.Map<Data.Dao.ContactDetail, ContactDetail>(dao);
            //createdModel.ContactUid = contactUid;
            result.Result.Add(createdModel);

            _logger.LogInformation("Create: End");
            return result;
        }

        public async Task<ManagerResponse<ContactDetail>> CreateOrUpdate(ContactDetail model)
        {
            _logger.LogInformation("CreateOrUpdate: Begin");

            var result = new ManagerResponse<ContactDetail>
            {
                ResultStatus = Common.Enums.ManagerResponseResult.Success,
                Result = new List<ContactDetail>()
            };

            var created = false;
            var dao = await _unitOfWork.Query<Data.Dao.ContactDetail>()
                .Where(o => o.Contact.Uid == ContactUid)
                .FirstOrDefaultAsync(o => o.Uid == model.Uid);
            if (dao == null)
            {
                // create
                dao = _mapper.Map<ContactDetail, Data.Dao.ContactDetail>(model);

                var parentId = await TryGetParentId(ContactUid);
                if (parentId == null)
                {
                    _logger.LogWarning("CreateOrUpdate: End (Invalid ContactUid)");
                    result.ResultStatus = Common.Enums.ManagerResponseResult.ForeignKeyViolation;
                    return result;
                }
                dao.ContactId = parentId.Value;

                _unitOfWork.Add(dao);
                created = true;
            }
            else
            {
                // update
                dao.Description = model.Description;
                dao.Value = model.Value;
                dao.Type = model.Type;

                _unitOfWork.Update(dao);
            }
            await _unitOfWork.SaveChangesAsync();

            var createdOrUpdatedModel = _mapper.Map<Data.Dao.ContactDetail, ContactDetail>(dao);
            //createdOrUpdatedModel.ContactUid = contactUid;
            result.Result.Add(createdOrUpdatedModel);

            if (created)
            {
                _logger.LogInformation("Put: End (Created)");
                result.ResultStatus = Common.Enums.ManagerResponseResult.Created;
                return result;
            }
            else
            {
                _logger.LogInformation("Put: End (Updated)");
                result.ResultStatus = Common.Enums.ManagerResponseResult.Updated;
                return result;
            }
        }

        public async Task<ManagerResponse<ContactDetail>> Delete(Guid uid)
        {
            _logger.LogInformation("Delete: Begin");

            var result = new ManagerResponse<ContactDetail>
            {
                ResultStatus = Common.Enums.ManagerResponseResult.Success,
                Result = new List<ContactDetail>()
            };

            var dao = await _unitOfWork.Query<Data.Dao.ContactDetail>()
                .Where(o => o.Contact.Uid == ContactUid)
                .FirstOrDefaultAsync(o => o.Uid == uid);
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

            if (disposing)
            {
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

        ~ContactDetailsManager()
        {
            Dispose(false);
        }

        #endregion
    }
}
