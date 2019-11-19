// -----------------------------------------------------------------------
// <copyright file="Task.cs" company="AraCom IT Services AG">
// Copyright (c) AraCom IT Services AG. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace TodoWebProjekt.Models
{
    /// <summary>
    /// The Task propertys.
    /// </summary>
    public class Task
    {
        /// <summary>
        /// Gets or sets the Task id.
        /// </summary>
        [Key]
        public int TaskId { get; set; }

        /// <summary>
        /// Gets or sets the Task description to exsplain the todo.
        /// </summary>
        [RegularExpression(@"^[\s\S]{0,1000}$", ErrorMessage= "Maximun length must be 1000")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the title of the todo.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the user id to defien a todo owner.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the assign user id to assign a user to the task.
        /// </summary>
        public string AssignUserId { get; set; }

        /// <summary>
        /// Gets or sets the task Status (Doing / Done).
        /// </summary>
        public string ActiveStatus { get; set; }

        /// <summary>
        /// Gets or sets the task Status (Important / Trivial).
        /// </summary>
        public string ImportanceStatus { get; set; }
    }
}