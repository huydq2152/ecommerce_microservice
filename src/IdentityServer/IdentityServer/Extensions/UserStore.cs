using IdentityServer.Infrastructure.Entities;
using IdentityServer.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Extensions;

public class UserStore : UserStore<User, IdentityRole, IdentityContext>
{
    public UserStore(IdentityContext context, IdentityErrorDescriber describer = null)
        : base(context, describer)
    {
    }

    // override GetRolesAsync return role ids
    public override async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken = new CancellationToken())
    {
        var query = from userRole in Context.UserRoles
                    join role in Context.Roles on userRole.RoleId equals role.Id
                    where userRole.UserId.Equals(user.Id)
                    select role.Id; // select role Id
        return await query.ToListAsync(cancellationToken);
    }
}