using FluentValidation;
using LIMTIC.Application.Contracts.UserManagement;
using LIMTIC.Application.Services.UserManagement;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LIMTIC.Application.IOC
{
    public static class RegisterApplicationModule
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register application services here
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddScoped<IUsersManagementService, UsersManagementService>();

            return services;
        }
    }
}
