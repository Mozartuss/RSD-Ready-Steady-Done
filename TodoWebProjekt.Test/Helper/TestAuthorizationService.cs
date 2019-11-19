using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Task = TodoWebProjekt.Models.Task;

namespace TodoWebProjekt.Test.Helper
{
    public class TestAuthorizationService : IAuthorizationService
    {
        public AuthorizationResult NextResult { get; set; }
            = AuthorizationResult.Failed();

        public List<(ClaimsPrincipal user, object resource, IEnumerable<IAuthorizationRequirement> requirements)>
            AuthorizeCalls { get; }
            = new List<(ClaimsPrincipal user, object resource, IEnumerable<IAuthorizationRequirement> requirements)>();

        public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object resource,
            IEnumerable<IAuthorizationRequirement> requirements)
        {
            AuthorizeCalls.Add((user, resource, requirements));

            if (resource.GetType() != typeof(Task))
            {
                return System.Threading.Tasks.Task.FromResult(NextResult);
            }

            var model = (Task) resource;

            return model.UserId == user.FindFirstValue(ClaimTypes.NameIdentifier) && model.UserId != null
                ? System.Threading.Tasks.Task.FromResult(AuthorizationResult.Success())
                : System.Threading.Tasks.Task.FromResult(NextResult);
        }

        public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object resource, string policyName)
        {
            throw new NotImplementedException();
        }
    }
}