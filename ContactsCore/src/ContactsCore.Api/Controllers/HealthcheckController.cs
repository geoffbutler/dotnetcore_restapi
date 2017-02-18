using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ContactsCore.Business.Managers;

namespace ContactsCore.Api.Controllers
{
    [Route("api/[controller]")]
    public class HealthcheckController : Controller
    {   
        private readonly ILogger<HealthcheckController> _logger;
        private readonly HealthcheckManager _manager;

        public HealthcheckController(ILogger<HealthcheckController> logger,             
            HealthcheckManager manager)
        {
            _logger = logger;
            _manager = manager;            
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("Get: Begin");

            var result = await _manager.Get();

            _logger.LogInformation("Get: End");

            return Ok(result);
        }
    }
}
