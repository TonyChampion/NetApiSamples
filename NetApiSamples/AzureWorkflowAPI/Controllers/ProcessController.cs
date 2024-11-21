using AzureWorkflowCommon.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureWorkflowAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcessController : ControllerBase
    {
        private readonly IServiceBusService _serviceBusService;
        private readonly ITableStorageService _storageService;

        public ProcessController(IServiceBusService serviceBusServie, ITableStorageService storageService)
        {
            _serviceBusService = serviceBusServie ?? throw new ArgumentNullException(nameof(serviceBusServie));
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));    
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<string>> RunJob()
        {
            return Ok(await _serviceBusService.AddJobAsync(""));
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<string>> GetStatus(string jobId)
        {
            var status = await _storageService.GetStatusAsync(jobId);

            return status == "not found" ? NotFound() : Ok(status);
        }
    }
}
