// -----------------------------------------------------------------------
// <copyright file="ApplicationUser.cs" company="AraCom IT Services AG">
// Copyright (c) AraCom IT Services AG. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TodoWebProjekt.Models
{
    /// <summary>
    /// The user model.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Gets or sets the Users firstname.
        /// </summary>
        [PersonalData]
        [Required]
        [StringLength(20)]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets tHe user lastname.
        /// </summary>
        [PersonalData]
        [Required]
        [StringLength(20)]
        public string LastName { get; set; }

        /// <summary>
        /// Gets the users full name.
        /// </summary>
        public string FullName => $"{FirstName} {LastName}";

        /// <summary>
        /// Gets or sets the date of creation.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the profile picture.
        /// </summary>
        public ProfilePicture ProfilePicture { get; set; }
    }
}