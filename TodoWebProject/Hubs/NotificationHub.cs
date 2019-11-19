// -----------------------------------------------------------------------
// <copyright file="NotificationHub.cs" company="AraCom IT Services AG">
// Copyright (c) AraCom IT Services AG. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.SignalR;

namespace TodoWebProjekt.Hubs
{
    /// <summary>
    /// The Hub to send notifications if a user create a new todo.
    /// </summary>
    public class NotificationHub : Hub
    {
        /// <summary>
        /// The mothod which creates the notification.
        /// </summary>
        /// <param name="userName"> The full name of the user which creats the todo. </param>
        /// <param name="taskName"> The title of the todo. </param>
        public void PushNotification(string userName, string taskName)
        {
            Clients.All.SendAsync("sendToast", userName, taskName);
        }
    }
}