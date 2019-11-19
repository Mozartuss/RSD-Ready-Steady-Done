// -----------------------------------------------------------------------
// <copyright file="RegisterViewModel.cs" company="AraCom IT Services AG">
// Copyright (c) AraCom IT Services AG. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TodoWebProjekt.ViewModel
{
    /// <summary>
    /// The important parametesrs to register an new User.
    /// </summary>
    public class RegisterViewModel
    {
        /// <summary>
        /// Gets or sets the users first name.
        /// </summary>
        [Display(Name = "First name")]
        [PersonalData]
        [Required]
        [StringLength(20)]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the users last name.
        /// </summary>
        [Display(Name = "Last name")]
        [PersonalData]
        [Required]
        [StringLength(20)]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the user email which act as username.
        /// Todo: Verify account with email.
        /// </summary>
        [PersonalData]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the new password.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "min 5, max 50 letters")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the confirmation of the new password.
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user role (User or Admin).
        /// </summary>
        public bool Role { get; set; }
    }
}