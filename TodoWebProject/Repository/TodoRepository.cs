// -----------------------------------------------------------------------
// <copyright file="TodoRepository.cs" company="AraCom IT Services AG">
// Copyright (c) AraCom IT Services AG. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TodoWebProjekt.Models;
using TodoWebProjekt.ViewModel;
using Task = TodoWebProjekt.Models.Task;

namespace TodoWebProjekt.Repository
{
    /// <summary>
    /// The implementation of the ItodoRepository interface.
    /// </summary>
    public class TodoRepository : ITodoRepository
    {
        private readonly TODOContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="TodoRepository"/> class.
        /// </summary>
        /// <param name="context"> The TodoContext. </param>
        public TodoRepository(TODOContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public async Task<int> Delete(int? id)
        {
            var result = 0;

            if (_context == null)
            {
                return result;
            }

            var task = await _context.Task.FindAsync(id);
            var file = await _context.File.FirstOrDefaultAsync(i => i.TaskId == id).ConfigureAwait(true);

            if (task == null)
            {
                return result;
            }

            _context.Task.Remove(task);

            if (file != null)
            {
                _context.File.Remove(file);
            }

            result = await _context.SaveChangesAsync().ConfigureAwait(true);

            return result;
        }

        /// <inheritdoc/>
        public IQueryable<Task> GetAll(string searchString, bool isAdmin, string currentUserId)
        {
            var tasks = from t in _context.Task select t;

            if (!string.IsNullOrEmpty(searchString))
            {
                tasks = tasks.Where(s => s.Title.Contains(searchString));
            }

            if (!isAdmin)
            {
                tasks = tasks.Where(t =>
                    t.UserId == currentUserId ||
                    t.AssignUserId == currentUserId);
            }

            return tasks;
        }

        /// <inheritdoc/>
        public async Task<FileTaskViewModel> GetFileTaskViewModel(int? id)
        {
            var currentTask = await _context.Task.FirstOrDefaultAsync(t => t.TaskId == id).ConfigureAwait(true);
            var currentFile = await _context.File.FirstOrDefaultAsync(f => f.TaskId == id).ConfigureAwait(true);

            if (currentFile == null && currentTask == null)
            {
                return null;
            }

            if (currentFile == null)
            {
                var fileTaskViewModel = new FileTaskViewModel
                {
                    Task = currentTask,
                    AssignUserId = currentTask.AssignUserId,
                    Active = currentTask.ActiveStatus == "Doing" ? true : false,
                    Important = currentTask.ImportanceStatus == "Important" ? true : false,
                };

                return fileTaskViewModel;
            }
            else
            {
                var fileTaskViewModel = new FileTaskViewModel
                {
                    Task = currentTask,
                    File = currentFile,
                    AssignUserId = currentTask.AssignUserId,
                    Active = currentTask.ActiveStatus == "Doing" ? true : false,
                    Important = currentTask.ImportanceStatus == "Important" ? true : false,
                };

                return fileTaskViewModel;
            }
        }

        /// <inheritdoc/>
        public async Task<ProfilePicture> GetProfilePicture(string id)
        {
            return await _context.ProfilePictures.FirstOrDefaultAsync(t => t.UserId == id).ConfigureAwait(true);
            
            
        }

        /// <inheritdoc/>
        public SelectList GetPossibleAssignUsers(string currentUserId)
        {
            var users = _context.ApplicationUser.OrderBy(c => c.Id)
                .Select(x => new { x.Id, Value = x.FullName })
                .Where(x => x.Id != currentUserId);
            return currentUserId == null ? null : new SelectList(users, "Id", "Value");
        }

        /// <inheritdoc/>
        public async Task<int> AddFileTask(FileTaskViewModel fileTaskViewModel)
        {
            if (fileTaskViewModel == null)
            {
                return 0;
            }

            var task = fileTaskViewModel.Task;
            var file = fileTaskViewModel.File;
            _context.Task.Add(task);
            if (file != null)
            {
                file.TaskId = task.TaskId;
                _context.File.Add(file);
            }

            await _context.SaveChangesAsync().ConfigureAwait(true);
            return fileTaskViewModel.Task.TaskId;
        }

        /// <inheritdoc/>
        public FileTaskViewModel GetEditView(int? id)
        {
            if (id == null)
            {
                return null;
            }

            var task = _context.Task.Find(id);
            var file = _context.File.FirstOrDefault(t => t.TaskId == id);
            var users = _context.ApplicationUser.OrderBy(c => c.UserName)
                .Select(x => new { x.Id, Value = x.FullName })
                .Where(x => x.Id != task.UserId);

            var createViewModel = new FileTaskViewModel
            {
                Task = task,
                File = file,
                ApplicationUserList = new SelectList(users, "Id", "Value", task.AssignUserId),
                Active = task.ActiveStatus == "Doing" ? true : false,
                Important = task.ImportanceStatus == "Important" ? true : false,
                AssignUserId = task.AssignUserId,
            };
            return createViewModel;
        }

        /// <inheritdoc/>
        public bool DeleteFile(File file)
        {
            EntityEntry<File> result = null;

            if (file == null)
            {
                return false;
            }

            var delFile = _context.File.FirstOrDefault(t => t.TaskId == file.TaskId);
            if (delFile != null)
            {
                result = _context.File.Remove(delFile);
            }

            return result != null;
        }

        /// <inheritdoc/>
        public EntityState AddFile(File file)
        {
            if (file == null)
            {
                return 0;
            }

            var result = _context.File.Add(file);
            var state = result.State;
            return state;
        }

        /// <inheritdoc/>
        public async Task<int> Update(FileTaskViewModel fileTaskViewModel)
        {
            var result = 0;
            if (fileTaskViewModel == null)
            {
                return result;
            }

            _context.Task.Update(fileTaskViewModel.Task);
            result = await _context.SaveChangesAsync().ConfigureAwait(true);
            return result;
        }
    }
}