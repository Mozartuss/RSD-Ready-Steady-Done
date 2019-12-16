// -----------------------------------------------------------------------
// <copyright file="AccountController.cs" company="AraCom IT Services AG">
// Copyright (c) AraCom IT Services AG. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoWebProjekt.Authorization;
using TodoWebProjekt.Email;
using TodoWebProjekt.Models;
using TodoWebProjekt.ViewModel;

namespace TodoWebProjekt.Controllers
{
    /// <summary>
    /// The controller to manage all the account operations.
    /// </summary>
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly TODOContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="userManager"> Build-in manager to manage user. </param>
        /// <param name="signInManager"> Build-in manager to manage the login process. </param>
        /// <param name="roleManager"> The buuild-in manager to manage the user rolls. </param>
        /// <param name="emailSender"> The service to send the confirmation email. </param>
        /// <param name="context"> THe context file. </param>
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            TODOContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _context = context;
        }

        /// <summary>
        /// Gets the register view.
        /// </summary>
        /// <param name="returnUrl"> Go back to given URL. </param>
        /// <returns> The register view. </returns>
        public IActionResult Register(string returnUrl)
        {
            var model = new RegisterViewModel
            {
                ReturnUrl = returnUrl,
            };
            return View(model);
        }

        /// <summary>
        /// The post register process, add a new registered user default to the uer rolle.
        /// </summary>
        /// <param name="model"> The register view model. </param>
        /// <param name="profilePicture"> the users profile picture. </param>
        /// <returns> Redirect back to the Index view of the Todo controller. </returns>
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model, IFormFile profilePicture)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model == null)
            {
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Created = DateTime.Now,
            };

            if (profilePicture != null)
            {
                if (profilePicture.ContentType.IndexOf("image", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    ModelState.AddModelError(string.Empty, "This file is not an image");

                    return View(model);
                }

                if (model != null)
                {
                    user.ProfilePicture = new ProfilePicture
                    {
                        ApplicationUser = user,
                    };

                    await using var ms = new MemoryStream();
                    await profilePicture.CopyToAsync(ms);

                    // max size = 300kb
                    if (ms.Length > 307200)
                    {
                        ModelState.AddModelError(string.Empty, "The Profile picture is too large max size: 300kb");
                        return View(model);
                    }

                    user.ProfilePicture.Image = ms.ToArray();
                    user.ProfilePicture.ContentType = profilePicture.ContentType;
                    user.ProfilePicture.Filename = Path.GetRandomFileName();
                }
            }

            var result = await _userManager.CreateAsync(user, model.Password).ConfigureAwait(true);

            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token }, Request.Scheme);

                await _emailSender.SendEmailAsync(
                toEmail: model.Email,
                toName: $"{model.FirstName} {model.LastName}",
                fromEmail: "support@todo.de",
                fromName: "Todo Webapplication",
                subject: "Bestätigung ihres accounts",
                message: $"Please confirm your Email </br> <a href={confirmationLink}> VERIFY </a>",
                isHtml: true);

                // Add an Administrator
                if (model.Role)
                {
                    await _roleManager.CreateAsync(new IdentityRole { Name = Constants.AdminRole })
                        .ConfigureAwait(true);
                    await _userManager.AddToRoleAsync(user, Constants.AdminRole).ConfigureAwait(true);
                }

                TempData["IsEmailSent"] = true;
                TempData["Email"] = model.Email;
                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            ModelState.AddModelError(string.Empty, "Register failed");

            return View(model);
        }

        /// <summary>
        /// Gets the login view.
        /// </summary>
        /// <param name="returnUrl"> The return adress after login. </param>
        /// <returns> The login view. </returns>
        public IActionResult Login(string returnUrl)
        {
            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
            };
            return View(model);
        }

        /// <summary>
        /// Here you can login with a provider (Google, Twitter, Facebook).
        /// </summary>
        /// <param name="provider"> The specific login provider. </param>
        /// <param name="returnUrl"> The return adress after login. </param>
        /// <returns> ChallangeResult. </returns>
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        /// <summary>
        /// The Backend of login or register with a new account with an external provider ( Facebook, Google, Twitter).
        /// </summary>
        /// <param name="returnUrl"> The current url where you start to login. </param>
        /// <param name="remoteError"> error from the provider. </param>
        /// <returns> Views. </returns>
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl ??= Url.Content("~/");

            var loginViewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl,
            };
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from the external provider: {remoteError}");

                return View("Login", loginViewModel);
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "Error loading external login information:");

                return View("Login", loginViewModel);
            }

            ApplicationUser user = null;
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(ClaimTypes.Name);
            var provider = info.ProviderDisplayName;

            // Check if you have an email and confirmed it.
            if ((provider == "Twitter") && name != null)
            {
                user = await _userManager.FindByNameAsync(name);

                if (user != null && user.Email != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet !");
                    return View("Login", loginViewModel);
                }
            }

            if ((provider == "Facebook") && name != null)
            {
                user = await _userManager.FindByNameAsync(name.Replace(" ", "_"));

                if (user != null && user.Email != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet !");
                    return View("Login", loginViewModel);
                }
            }

            if (provider == "Google" && email != null)
            {
                user = await _userManager.FindByEmailAsync(email);

                if (user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet !");
                    return View("Login", loginViewModel);
                }
            }

            // Login if user was found.
            if (user != null)
            {
                await _userManager.AddLoginAsync(user, info);
                var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
                if (signInResult.Succeeded)
                {
                    return LocalRedirect(returnUrl);
                }
            }

            // Create new Account.
            else
            {
                IdentityResult registerResult = IdentityResult.Failed();

                if (provider == "Facebook" && name != null)
                {
                    user = await _userManager.FindByNameAsync(name);

                    if (user == null)
                    {
                        string userName = info.Principal.FindFirstValue(ClaimTypes.Name).Replace(" ", "_");
                        user = new ApplicationUser
                        {
                            UserName = userName,
                            FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName),
                            LastName = info.Principal.FindFirstValue(ClaimTypes.Surname),
                            Created = DateTime.Now,
                        };

                        if (info.Principal.FindFirstValue(ClaimTypes.Email) != null)
                        {
                            user.Email = info.Principal.FindFirstValue(ClaimTypes.Email);
                        }

                        registerResult = await _userManager.CreateAsync(user);
                    }

                    if (info.Principal.HasClaim(c => c.Type == "profilePicture"))
                    {
                        await _userManager.AddClaimAsync(user, info.Principal.FindFirst("profilePicture"));
                    }
                }

                if (provider == "Twitter" && name != null)
                {
                    user = await _userManager.FindByNameAsync(name);

                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Name),
                            FirstName = string.Empty,
                            LastName = string.Empty,
                            Created = DateTime.Now,
                        };

                        if (info.Principal.FindFirstValue("display-name") != null)
                        {
                            var fullname = info.Principal.FindFirstValue("display-name");
                            user.FirstName = fullname.Split(null).First();
                            user.LastName = fullname.Split(null).Last();
                        }

                        if (info.Principal.FindFirstValue(ClaimTypes.Email) != null)
                        {
                            user.Email = info.Principal.FindFirstValue(ClaimTypes.Email);
                        }

                        registerResult = await _userManager.CreateAsync(user);

                        if (info.Principal.HasClaim(c => c.Type == "profilePicture"))
                        {
                            await _userManager.AddClaimAsync(user, info.Principal.FindFirst("profilePicture"));
                        }
                    }
                }

                if (provider == "Google" && email != null)
                {
                    user = await _userManager.FindByEmailAsync(email);

                    if (user == null)
                    {
                        var lowerCaseFullName = info.Principal.FindFirstValue(ClaimTypes.Name);

                        if (lowerCaseFullName != null && lowerCaseFullName.Split(null).Count() > 1)
                        {
                            string fullName = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(lowerCaseFullName.ToLower());
                            user = new ApplicationUser
                            {
                                UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                                Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                                FirstName = fullName.Split(null).First(),
                                LastName = fullName.Split().Last(),
                                Created = DateTime.Now,
                            };
                        }
                        else
                        {
                            user = new ApplicationUser
                            {
                                UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                                Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                                FirstName = string.Empty,
                                LastName = string.Empty,
                                Created = DateTime.Now,
                            };
                        }

                        registerResult = await _userManager.CreateAsync(user);

                        if (registerResult.Succeeded)
                        {
                            if (info.Principal.HasClaim(c => c.Type == "profilePicture"))
                            {
                                await _userManager.AddClaimAsync(user, info.Principal.FindFirst("profilePicture"));
                            }
                        }
                    }
                }

                // Facebook and Twitter have no email so you need no confirmation.
                if (registerResult.Succeeded && info.Principal.FindFirstValue(ClaimTypes.Email) == null)
                {
                    await _userManager.AddLoginAsync(user, info);
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }

                // Google give the user email so we need to confirm this one.
                else if (registerResult.Succeeded && info.Principal.FindFirstValue(ClaimTypes.Email) != null)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token }, Request.Scheme);

                    await _emailSender.SendEmailAsync(
                    toEmail: user.Email,
                    toName: user.FirstName != string.Empty && user.LastName != string.Empty ? user.FullName : user.UserName,
                    fromEmail: "support@todo.de",
                    fromName: "Todo Webapplication",
                    subject: "Bestätigung ihres accounts",
                    message: $"Please confirm your Email </br> <a href={confirmationLink}> VERIFY </a>",
                    isHtml: true);

                    TempData["IsEmailSent"] = true;
                    TempData["Email"] = user.Email;
                    return RedirectToAction("Login");
                }
            }

            return RedirectToAction("AccountError", "Account", new
            {
                firstLine = "Authentication",
                secondLine = "problem",
            });
        }

        /// <summary>
        /// The login process which checks if the user is in the database or know the correct password.
        /// </summary>
        /// <param name="model"> The Login view model. </param>
        /// <returns>If you comlete the login  process you will be redirect to the Index view of the todo controller. </returns>
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (model != null)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null && !user.EmailConfirmed && (await _userManager.CheckPasswordAsync(user, model.Password)))
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet !");
                    return View(model);
                }

                var result =
                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false)
                        .ConfigureAwait(true);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Todo");
                }
            }

            ModelState.AddModelError("Error", "Invalid Login Attempt");

            return View(model);
        }

        /// <summary>
        /// The Logout process where the current login user will bo loged out and the authentication cookie delete.
        /// </summary>
        /// <returns> Redirect to the Index view from the todo controller. </returns>
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync().ConfigureAwait(true);
            return RedirectToAction("Index", "Todo");
        }

        /// <summary>
        /// Generate the profile view model.
        /// </summary>
        /// <param name="id"> The user Id. </param>
        /// <returns>The profile view model.</returns>
        public async Task<IActionResult> Profile(string id)
        {
            var currentUser = await _userManager.FindByIdAsync(id).ConfigureAwait(true);
            var currentUserClaims = await _userManager.GetClaimsAsync(currentUser).ConfigureAwait(true);
            var currentUserRoles = await _userManager.GetRolesAsync(currentUser).ConfigureAwait(true);
            var currentProfilePic = await _context.ProfilePictures.FirstOrDefaultAsync(p => p.UserId == id);
            var provider = User.FindFirst(ClaimTypes.AuthenticationMethod)?.Value;

            if (currentUser != await _userManager.GetUserAsync(User).ConfigureAwait(true))
            {
                return RedirectToAction("Index", "Todo");
            }

            ProfileViewModel model = null;

            if (currentProfilePic != null)
            {
                model = new ProfileViewModel
                {
                    Id = currentUser.Id,
                    FirstName = currentUser.FirstName,
                    LastName = currentUser.LastName,
                    Created = currentUser.Created,
                    Email = currentUser.Email,
                    Mobile = currentUser.PhoneNumber,
                    Claims = currentUserClaims.Select(c => c.Value).ToList(),
                    Roles = currentUserRoles,
                    Provider = provider,
                    ProfilePicture = currentProfilePic,
                };
            }
            else
            {
                model = new ProfileViewModel
                {
                    Id = currentUser.Id,
                    FirstName = currentUser.FirstName,
                    LastName = currentUser.LastName,
                    Created = currentUser.Created,
                    Email = currentUser.Email,
                    Mobile = currentUser.PhoneNumber,
                    Claims = currentUserClaims.Select(c => c.Value).ToList(),
                    Provider = provider,
                    Roles = currentUserRoles,
                };
            }

            return PartialView("_ProfileModal", model);
        }

        /// <summary>
        /// Update the user Profile if the user change something.
        /// </summary>
        /// <param name="profileViewModel"> THe profile view modell. </param>
        /// <returns> Redirect to the Index view of the todo controller. </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel profileViewModel)
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

            if (profileViewModel == null)
            {
                return BadRequest();
            }

            var provider = User.FindFirstValue(ClaimTypes.AuthenticationMethod);

            var user = await _userManager.FindByIdAsync(profileViewModel.Id);

            var sameEmail = profileViewModel.Email == user.Email;

            if (user == null)
            {
                return BadRequest("Cannot find user");
            }

            user.Email = profileViewModel.Email;
            user.FirstName = profileViewModel.FirstName;
            user.LastName = profileViewModel.LastName;
            user.PhoneNumber = profileViewModel.Mobile;
            if (profileViewModel.Password != null && provider == null)
            {
                var passResult = await _userManager.ChangePasswordAsync(
                    user,
                    profileViewModel.OldPassword,
                    profileViewModel.Password);

                if (!passResult.Succeeded)
                {
                    ModelState.AddModelError("evry", "Password change failed");
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

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                if (profileViewModel.Email != null && !sameEmail)
                {
                    user.EmailConfirmed = false;

                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token }, Request.Scheme);

                    await _emailSender.SendEmailAsync(
                    toEmail: profileViewModel.Email,
                    toName: profileViewModel.FirstName != string.Empty && profileViewModel.LastName != string.Empty ? profileViewModel.FullName : user.UserName,
                    fromEmail: "support@todo.de",
                    fromName: "Todo Webapplication",
                    subject: "Bestätigung ihres accounts",
                    message: $"Please confirm your Email </br> <a href={confirmationLink}> VERIFY </a>",
                    isHtml: true);

                    TempData["Email"] = profileViewModel.Email;
                    TempData["IsEmailSent"] = true;
                }

                return Json(new { status = "success" });
            }
            else
            {
                ModelState.AddModelError("evry", "Update failed");
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

        /// <summary>
        /// Gets the access denied view.
        /// </summary>
        /// <param name="firstLine"> The first line of the error message. </param>
        /// <param name="secondLine"> The second line of the error messaage. </param>
        /// <returns> The access denied view. </returns>
        public IActionResult AccountError(string firstLine, string secondLine)
        {
            return View(model: new AccountErrorViewModel { FirstLine = firstLine, SecondLine = secondLine });
        }

        /// <summary>
        /// Confirm the Email if link was clicked.
        /// </summary>
        /// <param name="userId"> The uniqe user Id. </param>
        /// <param name="token"> The providers authentication token. </param>
        /// <returns> View. </returns>
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("index", "todo");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return BadRequest($"The User ID {userId} is invalid");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                TempData["EmailConfirmation"] = true;
                return RedirectToAction("Index", "todo");
            }

            return RedirectToAction("AccountError", new
            {
                firstLine = "Emailverification",
                secondLine = "failed",
            });
        }
    }
}