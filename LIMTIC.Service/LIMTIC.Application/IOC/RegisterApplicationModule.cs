using FluentValidation;
using LIMTIC.Application.Contracts.UserManagement;
using LIMTIC.Application.Services.UserManagement;
using LIMTIC.Application.Services;
using LIMTIC.Application.Services.Auth;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using LIMTIC.Application.Contracts.Auth;

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

            return services;
        }
    }
}
