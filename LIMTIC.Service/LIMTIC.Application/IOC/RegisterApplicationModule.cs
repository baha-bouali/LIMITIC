using System.Reflection;
using FluentValidation;
using LIMTIC.Application.Abstractions.Auth;
using LIMTIC.Application.Abstractions.Profiles;
using LIMTIC.Application.Abstractions.UserManagement;
using LIMTIC.Application.Services.Auth;
using LIMTIC.Application.Services.Profiles;
using LIMTIC.Application.Services.UserManagement;
using Microsoft.Extensions.DependencyInjection;

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

            // Profile services
            services.AddScoped<IResearcherProfileService, ResearcherProfileService>();
            services.AddScoped<IPhDStudentProfileService, PhDStudentProfileService>();
            services.AddScoped<IMasterianProfileService, MasterianProfileService>();

            return services;
        }
    }
}
