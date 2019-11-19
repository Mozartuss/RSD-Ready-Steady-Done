// -----------------------------------------------------------------------
// <copyright file="TaskOperations.cs" company="AraCom IT Services AG">
// Copyright (c) AraCom IT Services AG. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace TodoWebProjekt.Authorization
{
    /// <summary>
    /// The different operation Types where you use the authorization.
    /// </summary>
    public class TaskOperations : IAuthorizationRequirement
    {
        /// <summary>
        /// Gets or sets the Update operation authorization requirement.
        /// </summary>
        public static OperationAuthorizationRequirement Update { get; set; } = new OperationAuthorizationRequirement { Name = Constants.UpdateOperation };

        /// <summary>
        /// Gets or sets the Delete operation authorization requirement.
        /// </summary>
        public static OperationAuthorizationRequirement Delete { get; set; } = new OperationAuthorizationRequirement { Name = Constants.DeleteOperation };
    }
}