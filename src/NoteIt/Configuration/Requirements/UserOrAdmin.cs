using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoteIt.Configuration.Requirements
{
    public class UserOrAdmin : AuthorizationHandler<UserOrAdmin>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserOrAdmin requirement)
        {
            if(context.User.IsInRole("User"))
                context.Succeed(requirement);
            if(context.User.IsInRole("Admin"))
                context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
