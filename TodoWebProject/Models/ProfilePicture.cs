// -----------------------------------------------------------------------
// <copyright file="ProfilePicture.cs" company="AraCom IT Services AG">
// Copyright (c) AraCom IT Services AG. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace TodoWebProjekt.Models
{
    /// <summary>
    /// The table which contains the user Profile picture.
    /// </summary>
    public class ProfilePicture
    {
        /// <summary>
        /// Gets or sets the specific ProfilePictureId.
        /// </summary>
        [Key]
        public int ProfilePictureId { get; set; }

        /// <summary>
        /// Gets or sets the byte array which contains the image.
        /// </summary>
        public byte[] Image { get; set; }

        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// Gets or sets tHe content type of the image.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the id of the User which contains the Image.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets a link to the Task.
        /// </summary>
        public ApplicationUser ApplicationUser { get; set; }
    }
}
