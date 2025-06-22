using DinkToPdf;
using DinkToPdf.Contracts;
using HotChocolate.Subscriptions;
using LaundryCleaning.Service.Auth.Services.Implementations;
using LaundryCleaning.Service.Auth.Services.Interfaces;
using LaundryCleaning.Service.Common.Handlers;
using LaundryCleaning.Service.Common.Models.Messages;
using LaundryCleaning.Service.Data;
using LaundryCleaning.Service.Download;
using LaundryCleaning.Service.GraphQL.Files.Services.Implementations;
using LaundryCleaning.Service.GraphQL.Files.Services.Interfaces;
using LaundryCleaning.Service.GraphQL.Roles.Services.Implementations;
using LaundryCleaning.Service.GraphQL.Roles.Services.Interfaces;
using LaundryCleaning.Service.GraphQL.Users.Services.Implementations;
using LaundryCleaning.Service.GraphQL.Users.Services.Interfaces;
using LaundryCleaning.Service.Services.Background;
using LaundryCleaning.Service.Services.Dispatcher;
using LaundryCleaning.Service.Services.Implementations;
using LaundryCleaning.Service.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace LaundryCleaning.Service.Extensions
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services)
        {
            #region Services
            services.AddScoped<DatabaseSeeder>();
            services.AddScoped<IInvoiceNumberService, InvoiceNumberService>();
            services.AddScoped<IPasswordHasher<object>, PasswordHasher<object>>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IPublisherService, PublisherService>();
            services.AddHostedService<ReceivedBackgroundService>();
            services.AddSingleton<SecureDownloadHelper>();
            #endregion

            var context = new CustomAssemblyLoadContext();
            context.LoadUnmanagedLibrary(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Storages", "Lib", "libwkhtmltox", "libwkhtmltox.dll"));
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

            #region Dispatcher
            services.AddSingleton<SystemMessageDispatcher>();
            #endregion

            #region Handler
            services.AddScoped<ISystemMessageHandler<UserNotification>, UserNotificationHandler>();
            #endregion

            #region GraphQL Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserService, UserService>();
            #endregion

            return services;
        }
    }
}
