using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ContactsCore.Business.Managers;
using ContactsCore.Api.Helpers;

namespace ContactsCore.Api.Controllers
{
    [Route("api/[controller]")]
    public class ContactWithDetailsController : Controller
    {
        private const int GetMaxPageSize = int.MaxValue; // TODO

        private readonly ILogger<ContactWithDetailsController> _logger;
        private readonly PagingHeaderHelper _pagingHeaderHelper;
        private ContactWithDetailsManager _manager;

        public ContactWithDetailsController(ILogger<ContactWithDetailsController> logger,
            PagingHeaderHelper pagingHeaderHelper,
            ContactWithDetailsManager manager)
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

        ~ContactWithDetailsController()
        {
            Dispose(false);
        }

        #endregion
    }
}