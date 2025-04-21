using Dapr.Jobs;
using Dapr.Jobs.Models;
using Main.Models;

namespace Main.Services;

public class JobsService : IJobsService
{
    private readonly DaprJobsClient _daprJobsClient;

    public JobsService(DaprJobsClient daprJobsClient)
    {
        _daprJobsClient = daprJobsClient;
    }
    
    public async Task ScheduleJob(JobsDto job)
    {
        await _daprJobsClient.ScheduleJobAsync(
            job.JobName,
            DaprJobSchedule.FromDuration(TimeSpan.FromMinutes(1)),
            null,
            repeats: 1000);
    }

    public async Task<object> GetJob(string jobName)
    {
        var job = await _daprJobsClient.GetJobAsync(jobName);
        
        return job;
    }

    public async Task DeleteJob(string jobName)
    { 
        await _daprJobsClient.DeleteJobAsync(jobName);
    }
}