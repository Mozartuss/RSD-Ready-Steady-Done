// -----------------------------------------------------------------------
// <copyright file="TODOContext.cs" company="AraCom IT Services AG">
// Copyright (c) AraCom IT Services AG. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TodoWebProjekt.Models
{
    /// <summary>
    /// The context to acces the data layer.
    /// </summary>
    public class TODOContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TODOContext"/> class.
        /// </summary>
        public TODOContext()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TODOContext"/> class.
        /// </summary>
        /// <param name="options"> DbCOntextOptions. </param>
        public TODOContext(DbContextOptions<TODOContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the task table where all the informations abot the todo are saved.
        /// </summary>
        public DbSet<Task> Task { get; set; }

        /// <summary>
        /// Gets or sets the File table where thr todo image is saved as byte arrey.
        /// </summary>
        public DbSet<File> File { get; set; }

        /// <summary>
        /// Gets or sets the application user model where all the Login and register informations are saved.
        /// </summary>
        public DbSet<ApplicationUser> ApplicationUser { get; set; }

        /// <summary>
        /// Gets or sets the users profile picture.
        /// </summary>
        public DbSet<ProfilePicture> ProfilePictures { get; set; }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>()
                .HasOne(a => a.ProfilePicture)
                .WithOne(b => b.ApplicationUser)
                .HasForeignKey<ProfilePicture>(b => b.UserId);

            base.OnModelCreating(builder);
        }
    }
}