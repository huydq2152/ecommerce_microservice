using Shared.Common.Constants;

namespace Infrastructure.Extensions;

public class PermissionHelper
{
    public static string GetPermission(FunctionCode function, CommandCode command)
    {
        return string.Join(".", function, command);
    }
}