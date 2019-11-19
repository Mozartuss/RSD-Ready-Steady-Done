// -----------------------------------------------------------------------
// <copyright file="AdminAuthorizationHandler.cs" company="AraCom IT Services AG">
// Copyright (c) AraCom IT Services AG. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using TodoWebProjekt.Models;

namespace TodoWebProjekt.Authorization
{
    /// <summary>
    /// The admistration handler which checks the authorization.
    /// </summary>
    public class AdminAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Task>
    {
        /// <summary>
        /// Check if user is authorized.
        /// </summary>
        /// <param name="context"> The AuthorizationHandlerContext. </param>
        /// <param name="requirement"> The OperationAuthorizationRequirement. </param>
        /// <param name="model"> The Models.Task model. </param>
        /// <returns> Task complete(fail) or succeed. </returns>
        protected override System.Threading.Tasks.Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            Task model)
        {
            if (context != null && context.User == null)
            {
                return System.Threading.Tasks.Task.CompletedTask;
            }

            // Administrators can do anything.
            if (context != null && context.User.IsInRole(Constants.AdminRole))
            {
                context.Succeed(requirement);
            }

            return System.Threading.Tasks.Task.CompletedTask;
        }
    }
}