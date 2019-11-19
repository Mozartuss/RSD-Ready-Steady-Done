// -----------------------------------------------------------------------
// <copyright file="AuthMessageSenderOptions.cs" company="AraCom IT Services AG">
// Copyright (c) AraCom IT Services AG. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace TodoWebProjekt.Email
{
    /// <summary>
    /// Service to fetch secure email key.
    /// </summary>
    public class AuthMessageSenderOptions
    {
        /// <summary>
        /// Gets or sets the SendGrid key.
        /// </summary>
        public string SendGridKey { get; set; }
    }
}
