// -----------------------------------------------------------------------
// <copyright file="IEmailSender.cs" company="AraCom IT Services AG">
// Copyright (c) AraCom IT Services AG. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;

namespace TodoWebProjekt.Email
{
    /// <summary>
    /// The Email sender service interface.
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// The service to send an email to the receiver.
        /// </summary>
        /// <param name="toEmail"> The receiver Email. </param>
        /// <param name="toName"> The receiver name.</param>
        /// <param name="fromEmail"> The sender email. </param>
        /// <param name="fromName"> The sender name. </param>
        /// <param name="subject"> The subject of the email. </param>
        /// <param name="message"> the body content message of the email. </param>
        /// <param name="isHtml"> True if it is a HTML content. </param>
        /// <returns> The response status code. </returns>
        Task<System.Net.HttpStatusCode> SendEmailAsync(string toEmail, string toName, string fromEmail, string fromName, string subject, string message, bool isHtml);
    }
}
