// -----------------------------------------------------------------------
// <copyright file="AuthMessageSender.cs" company="AraCom IT Services AG">
// Copyright (c) AraCom IT Services AG. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace TodoWebProjekt.Email
{
    /// <summary>
    /// The sender calss to send the confirmation email.
    /// </summary>
    public class AuthMessageSender : IEmailSender
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthMessageSender"/> class.
        /// </summary>
        /// <param name="optionsAccessor"> The IOptions to access the SendGridKey. </param>
        public AuthMessageSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        /// <summary>
        /// Gets the send grid options.
        /// </summary>
        public AuthMessageSenderOptions Options { get; }

        /// <inheritdoc/>
        public async Task<System.Net.HttpStatusCode> SendEmailAsync(string toEmail, string toName, string fromEmail, string fromName, string subject, string message, bool isHtml)
        {
            var apiKey = Options.SendGridKey;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(fromEmail, fromName);
            var to = new EmailAddress(toEmail, toName);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, isHtml ? null : message, isHtml ? message : null);
            var response = await client.SendEmailAsync(msg);

            return response.StatusCode;
        }
    }
}
