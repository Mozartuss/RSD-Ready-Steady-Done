// -----------------------------------------------------------------------
// <copyright file="ITodoRepository.cs" company="AraCom IT Services AG">
// Copyright (c) AraCom IT Services AG. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TodoWebProjekt.Models;
using TodoWebProjekt.ViewModel;
using Task = TodoWebProjekt.Models.Task;

namespace TodoWebProjekt.Repository
{
    /// <summary>
    /// The toto repository interface.
    /// </summary>
    public interface ITodoRepository
    {
        /// <summary>
        /// Delete a todo with a specific id.
        /// </summary>
        /// <param name="id"> The TaskId. </param>
        /// <returns> 0 = false, 1 = true. </returns>
        Task<int> Delete(int? id);

        /// <summary>
        /// Get all the Tasks which contaisn nyour user id as userId or AssignId.
        /// </summary>
        /// <param name="searchString"> The current search string. </param>
        /// <param name="isAdmin"> A boolean to defien if your ara an admin. The admin cann see all the Todos and cann edit and delete all. </param>
        /// <param name="currentUserId"> THe id of the signed in user. </param>
        /// <returns> A Queryable which contains the list. </returns>
        IQueryable<Task> GetAll(string searchString, bool isAdmin, string currentUserId);

        /// <summary>
        /// Get the file task view Model of the specific item.
        /// </summary>
        /// <param name="id"> The Todo id. </param>
        /// <returns> The file task view model. </returns>
        Task<FileTaskViewModel> GetFileTaskViewModel(int? id);

        /// <summary>
        /// Get a list of all user names but not your own.
        /// </summary>
        /// <param name="currentUserId"> The singed in user id.  </param>
        /// <returns> The selected list which contsins the users. </returns>
        SelectList GetPossibleAssignUsers(string currentUserId);

        /// <summary>
        /// Add the new todo (with optional Image).
        /// </summary>
        /// <param name="fileTaskViewModel"> the file task view model. </param>
        /// <returns> Task id or 0. </returns>
        Task<int> AddFileTask(FileTaskViewModel fileTaskViewModel);

        /// <summary>
        /// Gets the edit view and all possible assignable user as list.
        /// </summary>
        /// <param name="id"> The specific task id. </param>
        /// <returns> The file task view model. </returns>
        FileTaskViewModel GetEditView(int? id);

        /// <summary>
        /// Delete only the file (image) in the file table.
        /// </summary>
        /// <param name="file"> The file you want to delete. </param>
        /// <returns> True if successfully delete, fasle when not. </returns>
        bool DeleteFile(File file);

        /// <summary>
        /// Add an new file and only the file.
        /// </summary>
        /// <param name="file"> The file you want to add. </param>
        /// <returns> 0 if failure and the id if success. </returns>
        EntityState AddFile(File file);

        /// <summary>
        /// Update the whole file tas view model (file and task).
        /// </summary>
        /// <param name="fileTaskViewModel"> The file task view model which one you want to update. </param>
        /// <returns> 0 if failure and 1 if success. </returns>
        Task<int> Update(FileTaskViewModel fileTaskViewModel);
    }
}