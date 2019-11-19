// -----------------------------------------------------------------------
// <copyright file="File.cs" company="AraCom IT Services AG">
// Copyright (c) AraCom IT Services AG. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace TodoWebProjekt.Models
{
    /// <summary>
    /// Defines the File table at the database.
    /// </summary>
    public class File
    {
        /// <summary>
        /// Gets or sets the specific FileId.
        /// </summary>
        public int FileId { get; set; }

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
        /// Gets or sets the id of the task which contains the Image.
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        /// Gets or sets a link to the Task.
        /// </summary>
        public virtual Task Task { get; set; }
    }
}