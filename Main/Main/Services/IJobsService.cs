using Main.Models;

namespace Main.Services;

public interface IJobsService
{
    Task ScheduleJob(JobsDto job);
    
    Task <object> GetJob(string jobName);
    
    Task DeleteJob(string jobName);
}