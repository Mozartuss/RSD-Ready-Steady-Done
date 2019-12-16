// -----------------------------------------------------------------------
// <copyright file="Startup.cs" company="AraCom IT Services AG">
// Copyright (c) AraCom IT Services AG. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TodoWebProjekt.Authorization;
using TodoWebProjekt.Email;
using TodoWebProjekt.Helper;
using TodoWebProjekt.Hubs;
using TodoWebProjekt.Models;
using TodoWebProjekt.Repository;

namespace TodoWebProjekt
{
    /// <summary>
    /// The startup class is the first called class and sets all the properties for the application.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration"> IConfiguration. </param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        ///  Gets the application configuration properties.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"> IServiceCollection. </param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TODOContext>(options =>
            {
                IConfigurationSection dbConnectionString =
                    Configuration.GetSection("Todos");

                options.UseSqlServer(dbConnectionString["DbConnectionString"]);
            });

            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddMvc();

            // Login
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireDigit = false;
                }).AddRoleManager<RoleManager<IdentityRole>>()
                .AddEntityFrameworkStores<TODOContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(1);
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            // External Login
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddGoogle(googleOptions =>
                {
                    IConfigurationSection googleAuthNSection =
                        Configuration.GetSection("Authentication:Google");

                    googleOptions.ClientId = googleAuthNSection["ClientId"];
                    googleOptions.ClientSecret = googleAuthNSection["ClientSecret"];

                    googleOptions.ClaimActions.MapJsonKey("profilePicture", "picture", "url");
                    googleOptions.SaveTokens = true;
                })
                .AddTwitter(twitterOptions =>
                {
                    IConfigurationSection twitterAuthNSection =
                        Configuration.GetSection("Authentication:Twitter");

                    twitterOptions.ConsumerKey = twitterAuthNSection["ConsumerKey"];
                    twitterOptions.ConsumerSecret = twitterAuthNSection["ConsumerSecret"];
                    twitterOptions.SaveTokens = true;
                    twitterOptions.RetrieveUserDetails = true;
                    twitterOptions.ClaimActions.MapJsonKey("display-name", "name");
                    twitterOptions.ClaimActions.MapJsonKey("profilePicture", "profile_image_url_https");
                })
                .AddFacebook(facebookOptions =>
                {
                    IConfigurationSection facebookAuthNSection =
                        Configuration.GetSection("Authentication:Facebook");

                    facebookOptions.AppId = facebookAuthNSection["AppId"];
                    facebookOptions.AppSecret = facebookAuthNSection["AppSecret"];

                    facebookOptions.Events.OnCreatingTicket = (context) =>
                    {
                        var picture = $"https://graph.facebook.com/{context.Principal.FindFirstValue(ClaimTypes.NameIdentifier)}/picture?type=large";
                        context.Identity.AddClaim(new Claim("profilePicture", picture));
                        return System.Threading.Tasks.Task.CompletedTask;
                    };
                    facebookOptions.UserInformationEndpoint = "https://graph.facebook.com/v5.0/me?fields=id,name,email";

                    facebookOptions.ClaimActions.MapJsonKey(ClaimTypes.DateOfBirth, "birthday");
                    facebookOptions.ClaimActions.MapJsonKey(ClaimTypes.Gender, "gender");
                    facebookOptions.ClaimActions.MapJsonSubKey("location", "location", "name");
                    facebookOptions.ClaimActions.MapJsonKey(ClaimTypes.Locality, "locale");
                    facebookOptions.ClaimActions.MapJsonKey("timezone", "timezone");
                })
                .AddCookie();

            // DSGVO
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Authorization
            services.AddControllers(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });

            services.AddScoped<ITodoRepository, TodoRepository>();

            services.AddScoped<IAuthorizationHandler, TaskOwnerAuthorizationHandler>();

            services.AddSingleton<IAuthorizationHandler, AdminAuthorizationHandler>();

            services.Configure<AuthMessageSenderOptions>(Configuration.GetSection("SendGridConfig"));

            services.AddTransient<IEmailSender, AuthMessageSender>();

            services.AddSignalR();

            services.AddMvc();

            services.AddDistributedMemoryCache();

            services.AddSession();

            services.AddHttpContextAccessor();
        }

        /// <summary>
        /// The Configure method is used to specify how the application will respond in each HTTP request.
        /// </summary>
        /// <param name="app"> IApplicationBuilder. </param>
        /// <param name="env"> IWebHostEnvironment. </param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseSession();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCookiePolicy();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Todo}/{action=Index}/{id?}");
                endpoints.MapHub<NotificationHub>("/notificationHub");
                endpoints.MapRazorPages();
            });
        }
    }
}