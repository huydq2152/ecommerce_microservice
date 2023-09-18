using Contracts.Configurations;
using Contracts.Services;
using Infrastructure.Configurations;
using MailKit.Net.Smtp;
using MimeKit;
using Serilog;
using Shared.Services.Email;

namespace Infrastructure.Services;

public class SmtpEmailService : ISmtpEmailService
{
    private readonly ILogger _logger;
    private readonly SmtpEmailSetting _emailSetting;
    private readonly SmtpClient _smtpClient;

    public SmtpEmailService(ILogger logger, SmtpEmailSetting emailSetting)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _emailSetting = emailSetting ?? throw new ArgumentNullException(nameof(emailSetting));
        _smtpClient = new SmtpClient();
    }

    public async Task SendEmailAsync(MailRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
        var emailMessage = new MimeMessage()
        {
            Sender = new MailboxAddress(_emailSetting.DisplayName, request.From ?? _emailSetting.From),
            Subject = request.Subject,
            Body = new BodyBuilder()
            {
                HtmlBody = request.Body
            }.ToMessageBody()
        };

        if (request.ToAddresses.Any())
        {
            foreach (var toAddress in request.ToAddresses)
            {
                emailMessage.To.Add(MailboxAddress.Parse(toAddress));
            }
        }
        else
        {
            var toAddress = request.ToAddress;
            emailMessage.To.Add(MailboxAddress.Parse(toAddress));
        }

        try
        {
            await _smtpClient.ConnectAsync(_emailSetting.SMTPServer, _emailSetting.Port, _emailSetting.UseSsl,
                cancellationToken);
            await _smtpClient.AuthenticateAsync(_emailSetting.Username, _emailSetting.Password, cancellationToken);
            await _smtpClient.SendAsync(emailMessage, cancellationToken);
            await _smtpClient.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.Error(e.Message, e);
        }
        finally
        {
            await _smtpClient.DisconnectAsync(true, cancellationToken);
            _smtpClient.Dispose();
        }
    }
}