using Microsoft.AspNetCore.Mvc;
using Shared.Common.Constants;

namespace Infrastructure.Identity.Authorization;

public class ClaimRequirementAttribute: TypeFilterAttribute
{
    public ClaimRequirementAttribute(FunctionCode function, CommandCode command) : base(typeof(ClaimRequirementFilter))
    {
        Arguments = new object[]
        {
            function, command
        };
    }
}