// -----------------------------------------------------------------------
// <copyright file="IndexViewModel.cs" company="AraCom IT Services AG">
// Copyright (c) AraCom IT Services AG. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using TodoWebProjekt.Helper;
using TodoWebProjekt.Models;

namespace TodoWebProjekt.ViewModel
{
    /// <summary>
    /// The index view model to get the Tasks as paginated list and the total length of the task list.
    /// </summary>
    public class IndexViewModel
    {
        /// <summary>
        /// Gets or sets the paginated task list.
        /// </summary>
        public PaginatedListCollection<Task> Tasks { get; set; }

        /// <summary>
        /// Gets or sets the length of the task list.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Gets or sets the NotImportentDoingPercent.
        /// </summary>
        public string NotImportentDoingPercent { get; set; }

        /// <summary>
        /// Gets or sets the NotImportentDonePercent.
        /// </summary>
        public string NotImportentDonePercent { get; set; }

        /// <summary>
        /// Gets or sets the ImportentDoingPercent.
        /// </summary>
        public string ImportentDoingPercent { get; set; }

        /// <summary>
        /// Gets or sets the ImportentDonePercent.
        /// </summary>
        public string ImportentDonePercent { get; set; }
    }
}