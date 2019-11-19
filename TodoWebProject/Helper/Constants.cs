// -----------------------------------------------------------------------
// <copyright file="Constants.cs" company="AraCom IT Services AG">
// Copyright (c) AraCom IT Services AG. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace TodoWebProjekt.Authorization
{
    /// <summary>
    /// The different authorizatioRequirement constants.
    /// </summary>
    public class Constants
    {
        // Authorization part

        /// <summary>
        /// The update constant.
        /// </summary>
        public static readonly string UpdateOperation = "Update";

        /// <summary>
        /// The delete constant.
        /// </summary>
        public static readonly string DeleteOperation = "Delete";

        /// <summary>
        /// The administration role constant.
        /// </summary>
        public static readonly string AdminRole = "Administrators";

        /// <summary>
        /// The user role constant.
        /// </summary>
        public static readonly string UserRole = "User";

        // Badges part

        /// <summary>
        /// The active badge.
        /// </summary>
        public static readonly string Active = "";

        /// <summary>
        /// The done badge.
        /// </summary>
        public static readonly string Done = "";

        /// <summary>
        /// The important badge.
        /// </summary>
        public static readonly string Important = "<span class='badge badge-danger' > Important</span>";

        /// <summary>
        /// The trivial badge.
        /// </summary>
        public static readonly string Trivial = "<span class='badge badge-light' > Trivial</span>";
    }
}