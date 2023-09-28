namespace Hangfire.API.Services.Interfaces;

public interface IBackgroundJobService
{
    string SendEmailContent(string email, string subject, string emailContent, DateTimeOffset enqueueAt);
}