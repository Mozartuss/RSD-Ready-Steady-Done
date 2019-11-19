// -----------------------------------------------------------------------
// <copyright file="ProfileViewModel.cs" company="AraCom IT Services AG">
// Copyright (c) AraCom IT Services AG. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TodoWebProjekt.ViewModel
{
    /// <summary>
    /// The profile view model which contains all the relevant profile infos.
    /// </summary>
    public class ProfileViewModel
    {
        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the users first name.
        /// </summary>
        [Display(Name = "First name")]
        [PersonalData]
        [Required]
        [StringLength(20)]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the user last name.
        /// </summary>
        [Display(Name = "Last name")]
        [PersonalData]
        [Required]
        [StringLength(20)]
        public string LastName { get; set; }

        /// <summary>
        /// Gets the users full name.
        /// </summary>
        public string FullName => $"{FirstName} {LastName}";

        /// <summary>
        /// Gets or sets the user email which act as username.
        /// </summary>
        [PersonalData]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the create date.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the user mobile number.
        /// Todo: Add two-factor authorization.
        /// </summary>
        [PersonalData]
        public string Mobile { get; set; }

        /// <summary>
        /// Gets or sets the current password.
        /// </summary>
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        /// <summary>
        /// Gets or sets the new password if you change it.
        /// </summary>
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "min 5, max 50 letters")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the confirmation of the new password.
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "New Password and confirmation password do not match")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Gets or sets all the different user claims.
        /// </summary>
        public List<string> Claims { get; set; }

        /// <summary>
        /// Gets or sets all the user roles.
        /// </summary>
        public IList<string> Roles { get; set; }
    }
}