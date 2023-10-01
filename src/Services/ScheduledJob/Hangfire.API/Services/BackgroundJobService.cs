using Contracts.ScheduleJobs;
using Contracts.Services;
using Hangfire.API.Services.Interfaces;
using Infrastructure.Services;
using Shared.Services.Email;
using ILogger = Serilog.ILogger;

namespace Hangfire.API.Services;

public class BackgroundJobService : IBackgroundJobService
{
    private readonly IScheduleJobService _scheduleJobService;
    private readonly ISmtpEmailService _smtpEmailService;
    private readonly ILogger _logger;

    public BackgroundJobService(IScheduleJobService scheduleJobService, ISmtpEmailService smtpEmailService,
        ILogger logger)
    {
        _scheduleJobService = scheduleJobService;
        _smtpEmailService = smtpEmailService;
        _logger = logger;
    }

    public IScheduleJobService ScheduleJobService => _scheduleJobService;

    public string? SendEmailContent(string email, string subject, string emailContent, DateTimeOffset enqueueAt)
    {
        var emailRequest = new MailRequest()
        {
            ToAddress = email,
            Subject = subject,
            Body = emailContent,
        };

        try
        {
            var jobId = _scheduleJobService.Schedule(() => _smtpEmailService.SendEmail(emailRequest), enqueueAt);
            _logger.Information($"Sent email to {email} with subject: {subject} - JobId: {jobId}");
            return jobId;
        }
        catch (Exception e)
        {
            _logger.Error(e.Message);
        }

        return null;
    }
}