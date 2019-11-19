// -----------------------------------------------------------------------
// <copyright file="TaskOwnerAuthorizationHandler.cs" company="AraCom IT Services AG">
// Copyright (c) AraCom IT Services AG. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using TodoWebProjekt.Models;

namespace TodoWebProjekt.Authorization
{
    /// <summary>
    /// Ther Handler to authorize only the Owner to edit or delete his Task.
    /// </summary>
    public class TaskOwnerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Task>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskOwnerAuthorizationHandler"/> class.
        /// </summary>
        /// <param name="userManager"> The apllication user manager. </param>
        public TaskOwnerAuthorizationHandler(UserManager<ApplicationUser>
            userManager)
        {
            _userManager = userManager;
        }

        /// <inheritdoc/>
        protected override System.Threading.Tasks.Task
            HandleRequirementAsync(
                AuthorizationHandlerContext context,
                OperationAuthorizationRequirement requirement,
                Task model)
        {
            if (context != null && (context.User == null || model == null))
            {
                return System.Threading.Tasks.Task.CompletedTask;
            }

            if (requirement != null && requirement.Name != Constants.UpdateOperation &&
                requirement.Name != Constants.DeleteOperation)
            {
                return System.Threading.Tasks.Task.CompletedTask;
            }

            if (context != null && model.UserId == _userManager.GetUserId(context.User) && model.UserId != null)
            {
                context.Succeed(requirement);
            }

            return System.Threading.Tasks.Task.CompletedTask;
        }
    }
}