using Shared.Services.Email;

namespace Contracts.Services;

public interface ISmtpEmailService: IEmailService<MailRequest>
{
    
}