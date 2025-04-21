using Main.Models;
using Main.Services;
using Microsoft.AspNetCore.Mvc;

namespace Main.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/jobs")]
public class JobsController : ControllerBase
{
    private readonly IJobsService _jobsService;

    public JobsController(IJobsService jobsService)
    {
        _jobsService = jobsService;
    }

    [HttpPost("schedule")]
    public async Task<IActionResult> ScheduleJob([FromBody] JobsDto job)
    {
        await _jobsService.ScheduleJob(job);
        
        return Ok();
    }

    [HttpGet("{jobName}")]
    public async Task<IActionResult> GetJob([FromRoute] string jobName)
    {
        var job = await _jobsService.GetJob(jobName);

        return new OkObjectResult(job);
    }

    [HttpDelete("{jobName}")]
    public async Task<IActionResult> DeleteJob([FromRoute] string jobName)
    {
        await _jobsService.DeleteJob(jobName);
            
        return Ok();
    }
}