using Contracts.ScheduleJobs;

namespace Hangfire.API.Services.Interfaces;

public interface IBackgroundJobService
{
    IScheduleJobService ScheduleJobService { get; }
    string SendEmailContent(string email, string subject, string emailContent, DateTimeOffset enqueueAt);
}