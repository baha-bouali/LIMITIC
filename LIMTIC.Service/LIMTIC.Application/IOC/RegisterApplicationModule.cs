using System.Reflection;
using FluentValidation;
using LIMTIC.Application.Abstractions.Auth;
using LIMTIC.Application.Abstractions.Events;
using LIMTIC.Application.Abstractions.UserManagement;
using LIMTIC.Application.Services.Auth;
using LIMTIC.Application.Services.Events;
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
            services.AddScoped<IEventsService, EventsService>();

            return services;
        }
    }
}
