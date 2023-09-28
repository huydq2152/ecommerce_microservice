using Basket.API.Services.Interface;

namespace Basket.API.Services;

public class EmailTemplateService
{
    private static readonly string _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
    private static readonly string _templateFolder = Path.Combine(_baseDirectory, "..\\..\\..\\EmailTemplates");

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