// -----------------------------------------------------------------------
// <copyright file="TodoController.cs" company="AraCom IT Services AG">
// Copyright (c) AraCom IT Services AG. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoWebProjekt.Authorization;
using TodoWebProjekt.Helper;
using TodoWebProjekt.Repository;
using TodoWebProjekt.ViewModel;
using File = TodoWebProjekt.Models.File;
using Task = TodoWebProjekt.Models.Task;

namespace TodoWebProjekt.Controllers
{
    /// <summary>
    /// The controller class which contaons all the CRUD operations.
    /// </summary>
    [Authorize]
    public class TodoController : Controller
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly ITodoRepository _todoRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TodoController"/> class.
        /// </summary>
        /// <param name="todoRepository"> The abstraction layer between the data access and the business logic layer of the Application. </param>
        /// <param name="authorizationService"> The service to authorize only the owner of an todo to change it for example. </param>
        public TodoController(ITodoRepository todoRepository, IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
            _todoRepository = todoRepository;
        }

        /// <summary>
        /// The Index/Main view where you see the tbale with all Todos, where you can sort, seach, and paginate the list.
        /// </summary>
        /// <param name="sortOrder"> The sortOrder you get from the cshtml file. </param>
        /// <param name="searchString"> THe serach word you get from the cshtml file. </param>
        /// <param name="currentFilter"> The currentFilter which contains the currentSerach so you can refresh the page and didn't lose the old search. </param>
        /// <param name="pageNumber"> The number that contains the page you are currently on. </param>
        /// <param name="pageSize"> The amount of items on a page. </param>
        /// <returns> The index view. </returns>
        public IActionResult Index(string sortOrder, string searchString, string currentFilter, int? pageNumber, int pageSize = 3)
         {
            var createViewModel = new IndexViewModel();

            ViewData["CurrentSort"] = sortOrder;
            ViewData["currentPageSize"] = pageSize;
            ViewData["TitleSortParam"] = string.IsNullOrEmpty(sortOrder) ? "Title_Desc" : "Title";
            ViewData["AuthorSortParam"] = string.IsNullOrEmpty(sortOrder) ? "Author_Desc" : "Author";
            ViewData["AssignSortParam"] = string.IsNullOrEmpty(sortOrder) ? "Assign_Desc" : "Assign";
            ViewData["ImportantFilter"] = string.IsNullOrEmpty(sortOrder) ? "Important" : "NotImportant";
            ViewData["StatusFilter"] = string.IsNullOrEmpty(sortOrder) ? "Doing" : "Done";


            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var isAdmin = User.IsInRole(Constants.AdminRole);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tasks = _todoRepository.GetAll(searchString, isAdmin, userId);

            switch (sortOrder)
            {
                case "Title_Desc":
                    {
                        tasks = tasks.OrderByDescending(t => t.Title);
                        break;
                    }

                case "Title":
                    {
                        tasks = tasks.OrderBy(t => t.Title);
                        ViewData["TitleSortParam"] = "Title_Desc";
                        break;
                    }

                case "Author":
                    {
                        tasks = tasks.OrderBy(t => t.UserId);
                        ViewData["AuthorSortParam"] = "Author_Desc";
                        break;
                    }

                case "Author_Desc":
                    {
                        tasks = tasks.OrderByDescending(t => t.UserId);
                        break;
                    }

                case "Assign":
                    {
                        tasks = tasks.OrderBy(t => t.AssignUserId);
                        ViewData["AssignSortParam"] = "Assign_Desc";
                        break;
                    }

                case "Assign_Desc":
                    {
                        tasks = tasks.OrderByDescending(t => t.AssignUserId);
                        break;
                    }

                case "Important":
                    {
                        tasks = tasks.Where(t => t.ImportanceStatus == "Important");
                        break;
                    }

                case "NotImportant":
                    {
                        tasks = tasks.Where(t => t.ImportanceStatus == string.Empty);
                        ViewData["ImportantFilter"] = "Important";
                        break;
                    }

                case "Doing":
                    {
                        tasks = tasks.Where(t => t.ActiveStatus == "Doing");
                        break;
                    }

                case "Done":
                    {
                        tasks = tasks.Where(t => t.ActiveStatus == "Done");
                        ViewData["StatusFilter"] = "Doing";
                        break;
                    }

                default:
                    {
                        tasks = tasks.OrderBy(i => i.TaskId);
                        break;
                    }
            }

            var taskLength = tasks.Count();

            if (pageSize == 0)
            {
                ViewData["currentPageSize"] = taskLength;
                pageSize = taskLength;
            }

            createViewModel.Tasks = PaginatedListCollection<Task>.Create(tasks.AsNoTracking(), pageNumber ?? 1, pageSize);
            createViewModel.Length = taskLength;

            return View(createViewModel);
        }

        /// <summary>
        /// Show the Details view.
        /// </summary>
        /// <param name="id"> The specific id of the item you want to watch. </param>
        /// <returns> The details view. </returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var details = await _todoRepository.GetFileTaskViewModel(id).ConfigureAwait(true);
            return details == null ? NotFound() : (IActionResult)View(details);
        }

        /// <summary>
        /// Send all the important data for the create view. And generate a User-list to assign user.
        /// </summary>
        /// <returns>The Create View. </returns>
        public IActionResult Create()
        {
            var createViewModel = new FileTaskViewModel
            {
                Task = new Task(),
                ApplicationUserList = _todoRepository.GetPossibleAssignUsers(User.FindFirstValue(ClaimTypes.NameIdentifier)),
            };

            return View(createViewModel);
        }

        /// <summary>
        /// Create a new Todod entry in the Database.
        /// </summary>
        /// <param name="fileTaskViewModel"> The view mode where the data-table and the image-table come to gether. </param>
        /// <param name="image"> The image input filehandler. </param>
        /// <returns> If all gone right you will be redirected to the Index view. </return>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FileTaskViewModel fileTaskViewModel, IFormFile image)
        {
            if (!ModelState.IsValid)
            {
                return View(fileTaskViewModel);
            }

            if (image != null)
            {
                if (fileTaskViewModel != null)
                {
                    fileTaskViewModel.File = new File
                    {
                        Task = fileTaskViewModel.Task,
                    };

                    await using var ms = new MemoryStream();
                    image.CopyTo(ms);
                    fileTaskViewModel.File.Image = ms.ToArray();
                    fileTaskViewModel.File.ContentType = image.ContentType;
                    fileTaskViewModel.File.Filename = Path.GetRandomFileName();
                }
            }

            if (fileTaskViewModel != null)
            {
                fileTaskViewModel.Task.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                fileTaskViewModel.Task.AssignUserId = fileTaskViewModel.AssignUserId;
                fileTaskViewModel.Task.ActiveStatus = fileTaskViewModel.Active ? "Doing" : "Done";
                fileTaskViewModel.Task.ImportanceStatus = fileTaskViewModel.Important ? "Important" : string.Empty;
            }

            var taskId = await _todoRepository.AddFileTask(fileTaskViewModel).ConfigureAwait(true);

            return taskId > 0 ? RedirectToAction("Index") : (IActionResult)NotFound();
        }

        /// <summary>
        /// Send all the important data for the edit view.
        /// </summary>
        /// <param name="id"> The specific task Id to get the right todo. </param>
        /// <returns> The edit view. </returns>
        public async Task<IActionResult> Edit(int? id)
        {
            var fileTaskViewModel = _todoRepository.GetEditView(id);
            if (fileTaskViewModel == null)
            {
                return BadRequest();
            }

            var isAuthorized =
                await _authorizationService.AuthorizeAsync(User, fileTaskViewModel.Task, TaskOperations.Update).ConfigureAwait(true);
            return !isAuthorized.Succeeded && !User.IsInRole(Constants.AdminRole)
                ? RedirectToAction("AccountError", "Account", new
                {
                    firstLine = "Authentication",
                    secondLine = "problem",
                })
                : (IActionResult)View(fileTaskViewModel);
        }

        /// <summary>
        /// The Post Edit methode which is called if you submit the changes.
        /// </summary>
        /// <param name="fileTaskViewModel"> The View Model where diffrent models and parameters are listed. </param>
        /// <param name="image"> The Image input from the edit page. </param>
        /// <returns> If all gone right you will be redirected back to the index view. </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FileTaskViewModel fileTaskViewModel, IFormFile image)
        {
            if (!ModelState.IsValid)
            {
                return View(fileTaskViewModel);
            }

            if (fileTaskViewModel == null)
            {
                return RedirectToAction("Index");
            }

            if (image != null)
            {
                if (fileTaskViewModel != null)
                {
                    var file = fileTaskViewModel.File;
                    if (file != null)
                    {
                        var del = _todoRepository.DeleteFile(file);
                        if (del)
                        {
                            await using (var ms = new MemoryStream())
                            {
                                image.CopyTo(ms);
                                file.Image = ms.ToArray();
                                file.ContentType = image.ContentType;
                                file.Filename = Path.GetRandomFileName();
                            }

                            file.TaskId = fileTaskViewModel.Task.TaskId;
                        }
                    }
                    else
                    {
                        file = new File
                        {
                            Task = fileTaskViewModel.Task,
                        };

                        await using var ms = new MemoryStream();
                        image.CopyTo(ms);
                        file.Image = ms.ToArray();
                        file.ContentType = image.ContentType;
                        file.Filename = Path.GetRandomFileName();
                        file.TaskId = fileTaskViewModel.Task.TaskId;
                    }

                    var entityState = _todoRepository.AddFile(file);
                    if (entityState == EntityState.Detached)
                    {
                        ModelState.AddModelError(string.Empty, "File Uploade Impossible");
                        return View(fileTaskViewModel);
                    }
                }
            }

            fileTaskViewModel.Task.AssignUserId = fileTaskViewModel.AssignUserId;
            fileTaskViewModel.Task.ActiveStatus = fileTaskViewModel.Active ? "Doing" : "Done";
            fileTaskViewModel.Task.ImportanceStatus = fileTaskViewModel.Important ? "Important" : string.Empty;

            var result = await _todoRepository.Update(fileTaskViewModel).ConfigureAwait(true);
            if (result == 0)
            {
                ModelState.AddModelError(string.Empty, "Failed to update the task!");
                return View(fileTaskViewModel);
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Delete a Todo Database entry with specific Id.
        /// </summary>
        /// <param name="id"> The uniq Id of the Todo you want to Delete. </param>
        /// <returns> If all gone right it will be refresh the index view. </returns>
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest("Id is null");
            }

            var fileTaskViewModel = await _todoRepository.GetFileTaskViewModel(id).ConfigureAwait(true);
            if (fileTaskViewModel == null)
            {
                return BadRequest("Wrong id");
            }

            var task = fileTaskViewModel.Task;

            var isAuthorized =
                await _authorizationService.AuthorizeAsync(User, task, TaskOperations.Update).ConfigureAwait(true);
            if (!isAuthorized.Succeeded && !User.IsInRole(Constants.AdminRole))
            {
                return RedirectToAction("AccountError", "Account", new
                {
                    firstLine = "Authentication",
                    secondLine = "problem",
                });
            }

            var result = await _todoRepository.Delete(id).ConfigureAwait(true);
            return result == 0 ? BadRequest("Delete dosn't work") : (IActionResult)RedirectToAction("Index");
        }
    }
}