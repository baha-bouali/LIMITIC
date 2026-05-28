using System.Reflection;
using FluentValidation;
using LIMTIC.Application.Abstractions.AuditLogs;
using LIMTIC.Application.Abstractions;
using LIMTIC.Application.Abstractions.Auth;
using LIMTIC.Application.Abstractions.Events;
using LIMTIC.Application.Abstractions.Contacts;
using LIMTIC.Application.Abstractions.Settings;
using LIMTIC.Application.Abstractions.Profiles;
using LIMTIC.Application.Abstractions.Publications;
using LIMTIC.Application.Abstractions.UserManagement;
using LIMTIC.Application.Services.AuditLogs;
using LIMTIC.Application.Services.Auth;
using LIMTIC.Application.Services.Events;
using LIMTIC.Application.Services.Contacts;
using LIMTIC.Application.Services.Profiles;
using LIMTIC.Application.Services.ResearchAxis;
using LIMTIC.Application.Services.UserManagement;
using LIMTIC.Application.Services.Settings;
using LIMTIC.Application.Services.Publications;
using Microsoft.Extensions.DependencyInjection;
using LIMTIC.Application.Abstractions.Publication;

namespace LIMTIC.Application.IOC
{
    public static class RegisterApplicationModule
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register application services here
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUsersManagementService, UsersManagementService>();
            services.AddScoped<IEventsService, EventsService>();
            services.AddScoped<ISettingsService, SettingsService>();
            services.AddScoped<IContactService, ContactService>();
            services.AddScoped<IAuditLogsService, AuditLogsService>();

            // Profile services
            services.AddScoped<IResearcherProfileService, ResearcherProfileService>();
            services.AddScoped<IPhDStudentProfileService, PhDStudentProfileService>();
            services.AddScoped<IMasterianProfileService, MasterianProfileService>();

            // Research axis service
            services.AddScoped<IResearchAxisService, ResearchAxisService>();
            services.AddScoped<IPublicationService, PublicationService>();

            return services;
        }
    }
}
