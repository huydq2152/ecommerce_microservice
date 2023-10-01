using Basket.API.Services.Interface;
using Shared.Configuration;

namespace Basket.API.Services;

public class EmailTemplateService
{
    protected readonly BackgroundJobSettings BackgroundJobSettings;
    
    private static readonly string _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
    private static readonly string _templateFolder = Path.Combine(_baseDirectory, "..\\..\\..\\EmailTemplates");

    public EmailTemplateService(BackgroundJobSettings backgroundJobSettings)
    {
        BackgroundJobSettings = backgroundJobSettings;
    }

    protected string ReadEmailTemplateContent(string templateEmailName, string format = "html")
    {
        var filePath = Path.Combine(_templateFolder, $"{templateEmailName}.{format}");
        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var streamReader = new StreamReader(fileStream);
        var emailText = streamReader.ReadToEnd();
        streamReader.Close();

        return emailText;
    }
}