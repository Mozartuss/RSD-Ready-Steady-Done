// -----------------------------------------------------------------------
// <copyright file="LoginViewModel.cs" company="AraCom IT Services AG">
// Copyright (c) AraCom IT Services AG. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace TodoWebProjekt.ViewModel
{
    /// <summary>
    /// The login view model.
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// Gets or sets the User email which act as the username.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the user password.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to Remember me.
        /// </summary>
        public bool RememberMe { get; set; }

        /// <summary>
        /// Gets or sets an url to go back afte login with any provider.
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}