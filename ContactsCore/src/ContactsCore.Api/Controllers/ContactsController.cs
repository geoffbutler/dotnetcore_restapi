using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ContactsCore.Api.Helpers;
using ContactsCore.Business.Managers;
using System.Net;

namespace ContactsCore.Api.Controllers
{
    [Route("api/[controller]")]
    public class ContactsController : Controller
    {
        private const int GetMaxPageSize = int.MaxValue; // TODO

        private readonly ILogger<ContactsController> _logger;
        private readonly PagingHeaderHelper _pagingHeaderHelper;
        private ContactsManager _manager;        

        public ContactsController(ILogger<ContactsController> logger,
            PagingHeaderHelper pagingHeaderHelper, 
            ContactsManager manager)
        {            
            _logger = logger;
            _pagingHeaderHelper = pagingHeaderHelper;
            _manager = manager;
        }
        
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]int pageNumber = 1, [FromQuery]int pageSize = 20)
        {
            _logger.LogInformation("Get: Begin");

            if (pageNumber <= 0)
            {
                _logger.LogWarning("Get: End (400 - pageNumber)");
                return BadRequest();
            }
            if (pageSize > GetMaxPageSize)
            {
                _logger.LogWarning("Get: End (400 - pageSize)");
                return BadRequest();
            }

            var result = await _manager.Get(pageNumber, pageSize);
            switch (result.ResultStatus)
            {
                case Common.Enums.ManagerResponseResult.Success:                 
                    _logger.LogInformation("Get: End (200)");
                    _pagingHeaderHelper.AddHeaders(Response, result.PageMeta);
                    return Ok(result.Result);
                default:
                    _logger.LogWarning("Get: End (400 - Error)");
                    return BadRequest(result.ErrorMessage ?? "An error occurred");
            }            
        }
        
        [HttpGet("{uid}")]
        public async Task<IActionResult> GetByUid(Guid uid)
        {
            _logger.LogInformation("GetByUid: Begin");

            if (uid == Guid.Empty)
            {
                _logger.LogWarning("GetByUid: End (400)");
                return BadRequest();
            }

            var result = await _manager.GetByUid(uid);
            switch (result.ResultStatus)
            {
                case Common.Enums.ManagerResponseResult.Success:
                    _logger.LogInformation("GetByUid: End (200)");
                    return Ok(result.Result.First());
                case Common.Enums.ManagerResponseResult.NotFound:
                    _logger.LogWarning("GetByUid: End (404)");
                    return NotFound();
                default:
                    _logger.LogWarning("GetByUid: End (400 - Error)");
                    return BadRequest(result.ErrorMessage ?? "An error occurred");
            }
        }        

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Model.Models.Contact model)
        {
            _logger.LogInformation("Post: Begin");
            
            if (!ModelState.IsValid)
            {                                
                _logger.LogWarning("Post: End (400)");
                return BadRequest(ModelState);
            }

            var result = await _manager.Create(model);
            switch (result.ResultStatus)
            {
                case Common.Enums.ManagerResponseResult.Success:
                    var createdModel = result.Result.First();
                    var locationUri = LocationUriHelper.GetLocationUriForPost(Request, createdModel.Uid);
                    _logger.LogInformation("Post: End (201)");
                    return Created(locationUri, createdModel);
                case Common.Enums.ManagerResponseResult.UniqueKeyViolation:
                    _logger.LogWarning("Post: End (409 - UniqueKeyViolation)");
                    return StatusCode((int)HttpStatusCode.Conflict, result.ErrorMessage);
                default:
                    _logger.LogWarning("Post: End (400 - Error)");
                    return BadRequest(result.ErrorMessage ?? "An error occurred");
            }
        }

        [HttpPut("{uid}")]
        public async Task<IActionResult> Put(Guid uid, [FromBody]Model.Models.Contact model)
        {
            _logger.LogInformation("Put: Begin");

            if (uid == Guid.Empty)
            {
                _logger.LogWarning("Put: End (400 - invalid uid)");
                return BadRequest();
            }
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Put: End (400)");                
                return BadRequest(ModelState);
            }

            if (uid != model.Uid)
            {
                _logger.LogWarning("Put: End (400 - uid mismatch)");                
                return BadRequest(ModelState);
            }

            var result = await _manager.CreateOrUpdate(model);            
            switch (result.ResultStatus)
            {
                case Common.Enums.ManagerResponseResult.Created:
                    var uri = LocationUriHelper.GetLocationUriForPut(Request);
                    _logger.LogInformation("Put: End (201)");
                    return Created(uri, result.Result.First());
                case Common.Enums.ManagerResponseResult.Updated:
                    _logger.LogInformation("Put: End (200)");
                    return Ok(result.Result.First());
                case Common.Enums.ManagerResponseResult.UniqueKeyViolation:
                    _logger.LogWarning("Put: End (409 - UniqueKeyViolation)");
                    return StatusCode((int)HttpStatusCode.Conflict, result.ErrorMessage);
                default:
                    _logger.LogWarning("Put: End (400 - Error)");
                    return BadRequest(result.ErrorMessage ?? "An error occurred");
            }
        }

        [HttpDelete("{uid}")]
        public async Task<IActionResult> Delete(Guid uid)
        {
            _logger.LogInformation("Delete: Begin");

            if (uid == Guid.Empty)
            {
                _logger.LogWarning("Delete: End (400 - invalid uid)");
                return BadRequest();
            }

            var result = await _manager.Delete(uid);
            switch (result.ResultStatus)
            {
                case Common.Enums.ManagerResponseResult.Success:
                    _logger.LogInformation("Delete: End (204)");
                    return NoContent();
                case Common.Enums.ManagerResponseResult.NotFound:
                    _logger.LogWarning("Delete: End (404)");
                    return NotFound();
                default:
                    _logger.LogWarning("Delete: End (400 - Error)");
                    return BadRequest(result.ErrorMessage ?? "An error occurred");
            }            
        }


        #region IDisposable

        // Flag: Has Dispose already been called?
        private bool _disposed;        

        // Protected implementation of Dispose pattern.
        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                if (_manager != null)
                {
                    _manager.Dispose();
                    _manager = null;
                }
            }

            // Free any unmanaged objects here.
            _disposed = true;

            // Call the base class implementation.
            base.Dispose(disposing);
        }

        ~ContactsController()
        {
            Dispose(false);
        }

        #endregion
    }
}
