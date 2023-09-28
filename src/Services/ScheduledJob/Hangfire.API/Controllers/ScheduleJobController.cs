using Hangfire.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.ScheduleJob;

namespace Hangfire.API.Controllers;

[ApiController]
[Route("api/scheduled-jobs")]
public class ScheduleJobController : ControllerBase
{
    private readonly IBackgroundJobService _backgroundJobService;

    public ScheduleJobController(IBackgroundJobService backgroundJobService)
    {
        _backgroundJobService = backgroundJobService;
    }

    [HttpPost]
    [Route("send-email-reminder-checkout-order")]
    public IActionResult SendReminderCheckoutOrderEmail([FromBody] ReminderCheckoutOrderDto input)
    {
        var jobId = _backgroundJobService.SendEmailContent(input.email, input.subject, input.emailContent,
            input.enqueueAt);
        return Ok(jobId);
    }
}