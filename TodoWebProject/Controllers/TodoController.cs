// -----------------------------------------------------------------------
// <copyright file="TodoController.cs" company="AraCom IT Services AG">
// Copyright (c) AraCom IT Services AG. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
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
        /// The Index/Main view.
        /// </summary>
        /// <returns> The index view. </returns>
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Load the Todo list with all optional filters...
        /// </summary>
        /// <param name="sortOrder"> The sortOrder you get from the cshtml file. </param>
        /// <param name="searchString"> THe serach word you get from the cshtml file. </param>
        /// <param name="currentFilter"> The currentFilter which contains the currentSerach so you can refresh the page and didn't lose the old search. </param>
        /// <param name="pageNumber"> The number that contains the page you are currently on. </param>
        /// <param name="pageSize"> The amount of items on a page. </param>
        /// <returns> The todo list partial view. </returns>
        [AllowAnonymous]
        public IActionResult LoadTodoList(string sortOrder, string searchString, string currentFilter, int? pageNumber, int pageSize)
        {
            var model = new IndexViewModel();

            if (pageSize == 0)
            {
                if (HttpContext.Session.GetInt32("CurrentPageSize") != null)
                {
                    pageSize = (int)HttpContext.Session.GetInt32("CurrentPageSize");
                }
                else
                {
                    pageSize = 5;
                }
            }

            ViewData["CurrentSort"] = sortOrder;
            HttpContext.Session.SetInt32("CurrentPageSize", pageSize);
            ViewData["TitleSortParam"] = string.IsNullOrEmpty(sortOrder) ? "Title_Desc" : "Title";
            ViewData["AuthorSortParam"] = string.IsNullOrEmpty(sortOrder) ? "Author_Desc" : "Author";
            ViewData["AssignSortParam"] = string.IsNullOrEmpty(sortOrder) ? "Assign_Desc" : "Assign";

            if (searchString == null)
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var isAdmin = User.IsInRole(Constants.AdminRole);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            IQueryable<Task> tasks = _todoRepository.GetAll(searchString, isAdmin, userId);

            var totalAmount = tasks.Count();
            var doing = tasks.Where(a => a.ActiveStatus == "Doing");
            var done = tasks.Where(a => a.ActiveStatus == "Done");

            double importentDoingPercent = doing.Where(a => a.ImportanceStatus == "Important").Count();
            double notImportentDoingPercent = doing.Where(a => a.ImportanceStatus == string.Empty).Count();
            double importentDonePercent = done.Where(a => a.ImportanceStatus == "Important").Count();
            double notImportentDonePercent = done.Where(a => a.ImportanceStatus == string.Empty).Count();

            model.ImportentDoingPercent = ((importentDoingPercent / totalAmount) * 100).ToString(CultureInfo.CreateSpecificCulture("en-GB")) + "%";
            model.NotImportentDoingPercent = ((notImportentDoingPercent / totalAmount) * 100).ToString(CultureInfo.CreateSpecificCulture("en-GB")) + "%";
            model.ImportentDonePercent = ((importentDonePercent / totalAmount) * 100).ToString(CultureInfo.CreateSpecificCulture("en-GB")) + "%";
            model.NotImportentDonePercent = ((notImportentDonePercent / totalAmount) * 100).ToString(CultureInfo.CreateSpecificCulture("en-GB")) + "%";

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
                        break;
                    }

                default:
                    {
                        tasks = tasks.OrderBy(i => i.TaskId);
                        break;
                    }
            }

            var taskLength = tasks.Count();

            if (pageSize == -1)
            {
                HttpContext.Session.SetInt32("CurrentPageSize", taskLength);
                pageSize = taskLength;
            }

            model.Tasks = PaginatedListCollection<Task>.Create(tasks.AsNoTracking(), pageNumber ?? 1, pageSize);
            model.Length = taskLength;

            return PartialView("_TodoList", model);
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
            details.AuthorProfilePicture = await _todoRepository.GetProfilePicture(details.Task.UserId);
            return details == null ? NotFound() : (IActionResult)PartialView("_DetailsModal", details);
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

            return PartialView("_CreateModal", createViewModel);
        }

        /// <summary>
        /// Create a new Todod entry in the Database.
        /// </summary>
        /// <param name="model"> The view mode where the data-table and the image-table come to gether. </param>
        /// <param name="image"> The image input filehandler. </param>
        /// <returns> If all gone right you will be redirected to the Index view. </return>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FileTaskViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    status = "failure",
                    formErrors = ModelState.Select(kvp => new
                    {
                        key = kvp.Key,
                        errors = kvp.Value.Errors.Select(e => e.ErrorMessage),
                    }),
                });
            }

            if (model.UploadImage != null)
            {
                if (model.UploadImage.ContentType.IndexOf("image", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    ModelState.AddModelError("UploadeImage", "This file is not an image");

                    return Json(new
                    {
                        status = "failure",
                        formErrors = ModelState.Select(kvp => new
                        {
                            key = kvp.Key,
                            errors = kvp.Value.Errors.Select(e => e.ErrorMessage),
                        }),
                    });
                }

                if (model != null)
                {
                    model.File = new File
                    {
                        Task = model.Task,
                    };

                    await using var ms = new MemoryStream();
                    model.UploadImage.CopyTo(ms);
                    model.File.Image = ms.ToArray();
                    model.File.ContentType = model.UploadImage.ContentType;
                    model.File.Filename = Path.GetRandomFileName();
                }
            }

            if (model != null)
            {
                model.Task.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                model.Task.AssignUserId = model.AssignUserId;
                model.Task.ActiveStatus = model.Active ? "Doing" : "Done";
                model.Task.ImportanceStatus = model.Important ? "Important" : string.Empty;
            }

            var taskId = await _todoRepository.AddFileTask(model).ConfigureAwait(true);

            return taskId > 0 ? Json(new { status = "success" }) : (IActionResult)NotFound();
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

            if (fileTaskViewModel.File != null && fileTaskViewModel.File.Image != null)
            {
                fileTaskViewModel.EmptyImage = false;
            }

            var isAuthorized =
                await _authorizationService.AuthorizeAsync(User, fileTaskViewModel.Task, TaskOperations.Update).ConfigureAwait(true);
            return !isAuthorized.Succeeded && !User.IsInRole(Constants.AdminRole)
                ? RedirectToAction("AccountError", "Account", new
                {
                    firstLine = "Authentication",
                    secondLine = "problem",
                })
                : (IActionResult)PartialView("_EditModal", fileTaskViewModel);
        }

        /// <summary>
        /// The Post Edit methode which is called if you submit the changes.
        /// </summary>
        /// <param name="model"> The View Model where diffrent models and parameters are listed. </param>
        /// <param name="uploadImage"> The Image input from the edit page. </param>
        /// <returns> If all gone right you will be redirected back to the index view. </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FileTaskViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    status = "failure",
                    formErrors = ModelState.Select(kvp => new
                    {
                        key = kvp.Key,
                        errors = kvp.Value.Errors.Select(e => e.ErrorMessage),
                    }),
                });
            }

            if (model == null)
            {
                return RedirectToAction("Index");
            }

            if (model.EmptyImage)
            {
                var file = model.File;
                if (file != null)
                {
                    var del = _todoRepository.DeleteFile(file);
                    if (!del)
                    {
                        ModelState.AddModelError("UploadeImage", "Not able to delete the Image");

                        return Json(new
                        {
                            status = "failure",
                            formErrors = ModelState.Select(kvp => new
                            {
                                key = kvp.Key,
                                errors = kvp.Value.Errors.Select(e => e.ErrorMessage),
                            }),
                        });
                    }
                }
            }

            if (model.UploadImage != null)
            {
                if (model != null)
                {
                    var file = model.File;
                    if (file != null)
                    {
                        var del = _todoRepository.DeleteFile(file);
                        if (del)
                        {
                            await using (var ms = new MemoryStream())
                            {
                                model.UploadImage.CopyTo(ms);
                                file.Image = ms.ToArray();
                                file.ContentType = model.UploadImage.ContentType;
                                file.Filename = Path.GetRandomFileName();
                            }

                            file.TaskId = model.Task.TaskId;
                        }
                    }
                    else
                    {
                        file = new File
                        {
                            Task = model.Task,
                        };

                        await using var ms = new MemoryStream();
                        model.UploadImage.CopyTo(ms);
                        file.Image = ms.ToArray();
                        file.ContentType = model.UploadImage.ContentType;
                        file.Filename = Path.GetRandomFileName();
                        file.TaskId = model.Task.TaskId;
                    }

                    var entityState = _todoRepository.AddFile(file);
                    if (entityState == EntityState.Detached)
                    {
                        ModelState.AddModelError("UploadeImage", "This file is not an image");

                        return Json(new
                        {
                            status = "failure",
                            formErrors = ModelState.Select(kvp => new
                            {
                                key = kvp.Key,
                                errors = kvp.Value.Errors.Select(e => e.ErrorMessage),
                            }),
                        });
                    }
                }
            }

            model.Task.AssignUserId = model.AssignUserId;
            model.Task.ActiveStatus = model.Active ? "Doing" : "Done";
            model.Task.ImportanceStatus = model.Important ? "Important" : string.Empty;

            var result = await _todoRepository.Update(model).ConfigureAwait(true);
            if (result == 0)
            {
                ModelState.AddModelError("Task", "Failed to update the task!");
                return Json(new
                {
                    status = "failure",
                    formErrors = ModelState.Select(kvp => new
                    {
                        key = kvp.Key,
                        errors = kvp.Value.Errors.Select(e => e.ErrorMessage),
                    }),
                });
            }

            return Json(new { status = "success" });
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
            return result == 0 ? BadRequest("Delete dosn't work") : (IActionResult)RedirectToAction("LoadTodoList");
        }

        /// <summary>
        /// Check a Todo Database entry with specific Id.
        /// </summary>
        /// <param name="id"> The uniq Id of the Todo you want to Delete. </param>
        /// <returns> If all gone right it will be refresh the index view. </returns>
        [HttpPost]
        public async Task<IActionResult> Check(int? id)
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

            if (fileTaskViewModel.Task.ActiveStatus == "Doing")
            {
                fileTaskViewModel.Task.ActiveStatus = "Done";
            }

            var result = await _todoRepository.Update(fileTaskViewModel).ConfigureAwait(true);
            return result == 0 ? BadRequest("Update dosn't work") : (IActionResult)RedirectToAction("LoadTodoList");
        }

        /// <summary>
        /// Uncheck a Todo Database entry with specific Id.
        /// </summary>
        /// <param name="id"> The uniq Id of the Todo you want to Delete. </param>
        /// <returns> If all gone right it will be refresh the index view. </returns>
        [HttpPost]
        public async Task<IActionResult> Uncheck(int? id)
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

            if (fileTaskViewModel.Task.ActiveStatus == "Done")
            {
                fileTaskViewModel.Task.ActiveStatus = "Doing";
            }

            var result = await _todoRepository.Update(fileTaskViewModel).ConfigureAwait(true);
            return result == 0 ? BadRequest("Update dosn't work") : (IActionResult)RedirectToAction("LoadTodoList");
        }
    }
}