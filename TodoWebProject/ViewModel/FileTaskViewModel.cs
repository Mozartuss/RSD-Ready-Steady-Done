// -----------------------------------------------------------------------
// <copyright file="FileTaskViewModel.cs" company="AraCom IT Services AG">
// Copyright (c) AraCom IT Services AG. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc.Rendering;
using TodoWebProjekt.Models;

namespace TodoWebProjekt.ViewModel
{
    /// <summary>
    /// A class which combine the Task and the File model.
    /// </summary>
    public class FileTaskViewModel
    {
        /// <summary>
        /// Gets or sets the task model.
        /// </summary>
        public Task Task { get; set; }

        /// <summary>
        /// Gets or sets the file model.
        /// </summary>
        public File File { get; set; }

        /// <summary>
        /// Gets or sets the Application List which contains all assignable user.
        /// </summary>
        public SelectList ApplicationUserList { get; set; }

        /// <summary>
        /// Gets or sets the assigned usser id.
        /// </summary>
        public string AssignUserId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to the active status.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to the important status.
        /// </summary>
        public bool Important { get; set; }
    }
}